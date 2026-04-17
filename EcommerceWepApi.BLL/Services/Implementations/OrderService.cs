using AutoMapper;
using EcommerceWepApi.BLL.DTOs.Common;
using EcommerceWepApi.BLL.DTOs.Order;
using EcommerceWepApi.BLL.Exceptions;
using EcommerceWepApi.BLL.Services.Interfaces;
using EcommerceWepApi.DAL.Models;
using EcommerceWepApi.DAL.Models.Enums;
using EcommerceWepApi.DAL.Repositories.Interfaces;

namespace EcommerceWepApi.BLL.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAdminLogService _adminLogService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IAdminLogService adminLogService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _adminLogService = adminLogService;
        }

        /// <summary>
        /// إنشاء طلب جديد من السلة
        /// </summary>
        public async Task<ApiResponse<OrderDto>> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            // جلب سلة المستخدم
            var cartItems = await _unitOfWork.Carts.GetAllWithIncludeAsync(
                c => c.UserId == userId, c => c.Product);

            var cartList = cartItems.ToList();
            if (!cartList.Any())
            {
                return ApiResponse<OrderDto>.FailureResponse("السلة فارغة");
            }

            // التحقق من توفر المنتجات
            foreach (var item in cartList)
            {
                if (item.Product.IsDeleted || !item.Product.IsActive)
                {
                    return ApiResponse<OrderDto>.FailureResponse(
                        $"المنتج '{item.Product.Name}' غير متاح حالياً");
                }

                if (item.Product.StockQuantity < item.Quantity)
                {
                    return ApiResponse<OrderDto>.FailureResponse(
                        $"الكمية المطلوبة من '{item.Product.Name}' غير متوفرة. المتاح: {item.Product.StockQuantity}");
                }
            }

            // حساب إجمالي السعر
            decimal totalPrice = cartList.Sum(c => c.Product.Price * c.Quantity);

            // تحويل طريقة الدفع
            if (!Enum.TryParse<PaymentMethod>(dto.PaymentMethod, true, out var paymentMethod))
            {
                return ApiResponse<OrderDto>.FailureResponse(
                    "طريقة الدفع غير صحيحة. الخيارات: CreditCard, DebitCard, PayPal");
            }

            // إنشاء الطلب
            var order = new Order
            {
                UserId = userId,
                TotalPrice = totalPrice,
                Status = OrderStatus.Pending,
                ShippingAddress = dto.ShippingAddress,
                ShippingCity = dto.ShippingCity,
                ShippingCountry = dto.ShippingCountry,
                OrderNotes = dto.OrderNotes
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            // إنشاء عناصر الطلب
            var orderItems = cartList.Select(c => new OrderItem
            {
                OrderId = order.Id,
                ProductId = c.ProductId,
                Quantity = c.Quantity,
                UnitPrice = c.Product.Price
            }).ToList();

            await _unitOfWork.OrderItems.AddRangeAsync(orderItems);

            // تقليل الكمية من المخزون
            foreach (var item in cartList)
            {
                item.Product.StockQuantity -= item.Quantity;
                _unitOfWork.Products.Update(item.Product);
            }

            // إنشاء سجل الدفع
            var payment = new Payment
            {
                OrderId = order.Id,
                Amount = totalPrice,
                Status = PaymentStatus.Pending,
                PaymentMethod = paymentMethod,
                TransactionId = Guid.NewGuid().ToString("N"),
                Reference = $"ORD-{order.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}"
            };

            await _unitOfWork.Payments.AddAsync(payment);

            // مسح السلة
            _unitOfWork.Carts.DeleteRange(cartList);

            await _unitOfWork.SaveChangesAsync();

            // جلب الطلب كامل مع البيانات المرتبطة
            var createdOrder = await _unitOfWork.Orders.GetFirstWithIncludeAsync(
                o => o.Id == order.Id,
                o => o.User,
                o => o.OrderItems);

            // جلب عناصر الطلب مع المنتجات
            var items = await _unitOfWork.OrderItems.GetAllWithIncludeAsync(
                oi => oi.OrderId == order.Id,
                oi => oi.Product);

            var orderDto = _mapper.Map<OrderDto>(createdOrder);
            orderDto.Items = _mapper.Map<List<OrderItemDto>>(items);

            return ApiResponse<OrderDto>.SuccessResponse(orderDto, "تم إنشاء الطلب بنجاح");
        }

        /// <summary>
        /// جلب طلبات المستخدم
        /// </summary>
        public async Task<ApiResponse<PaginatedResponse<OrderDto>>> GetUserOrdersAsync(
            int userId, PaginationParams paginationParams)
        {
            var (orders, totalCount) = await _unitOfWork.Orders.GetPagedAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize,
                predicate: o => o.UserId == userId,
                orderBy: o => o.CreatedAt,
                isDescending: true,
                o => o.User,
                o => o.OrderItems);

            var orderDtos = new List<OrderDto>();
            foreach (var order in orders)
            {
                var orderDto = _mapper.Map<OrderDto>(order);

                // جلب عناصر الطلب مع المنتجات
                var items = await _unitOfWork.OrderItems.GetAllWithIncludeAsync(
                    oi => oi.OrderId == order.Id,
                    oi => oi.Product);
                orderDto.Items = _mapper.Map<List<OrderItemDto>>(items);

                orderDtos.Add(orderDto);
            }

            var response = new PaginatedResponse<OrderDto>
            {
                Items = orderDtos,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalCount = totalCount
            };

            return ApiResponse<PaginatedResponse<OrderDto>>.SuccessResponse(response);
        }

        /// <summary>
        /// جلب تفاصيل طلب معين
        /// </summary>
        public async Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int orderId, int userId, bool isAdmin = false)
        {
            var order = await _unitOfWork.Orders.GetFirstWithIncludeAsync(
                o => o.Id == orderId,
                o => o.User,
                o => o.OrderItems);

            if (order == null)
            {
                throw new NotFoundException("الطلب", orderId);
            }

            // التحقق من الصلاحية - المستخدم يشوف طلباته فقط
            if (!isAdmin && order.UserId != userId)
            {
                throw new ForbiddenException("ليس لديك صلاحية لعرض هذا الطلب");
            }

            var orderDto = _mapper.Map<OrderDto>(order);

            // جلب عناصر الطلب مع المنتجات
            var items = await _unitOfWork.OrderItems.GetAllWithIncludeAsync(
                oi => oi.OrderId == orderId,
                oi => oi.Product);
            orderDto.Items = _mapper.Map<List<OrderItemDto>>(items);

            return ApiResponse<OrderDto>.SuccessResponse(orderDto);
        }

        /// <summary>
        /// جلب جميع الطلبات (أدمن)
        /// </summary>
        public async Task<ApiResponse<PaginatedResponse<OrderDto>>> GetAllOrdersAsync(
            PaginationParams paginationParams, string? statusFilter = null)
        {
            // بناء شرط التصفية
            System.Linq.Expressions.Expression<Func<Order, bool>>? predicate = null;

            if (!string.IsNullOrWhiteSpace(statusFilter) &&
                Enum.TryParse<OrderStatus>(statusFilter, true, out var status))
            {
                predicate = o => o.Status == status;
            }

            var (orders, totalCount) = await _unitOfWork.Orders.GetPagedAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize,
                predicate,
                orderBy: o => o.CreatedAt,
                isDescending: true,
                o => o.User,
                o => o.OrderItems);

            var orderDtos = new List<OrderDto>();
            foreach (var order in orders)
            {
                var orderDto = _mapper.Map<OrderDto>(order);

                var items = await _unitOfWork.OrderItems.GetAllWithIncludeAsync(
                    oi => oi.OrderId == order.Id,
                    oi => oi.Product);
                orderDto.Items = _mapper.Map<List<OrderItemDto>>(items);

                orderDtos.Add(orderDto);
            }

            var response = new PaginatedResponse<OrderDto>
            {
                Items = orderDtos,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalCount = totalCount
            };

            return ApiResponse<PaginatedResponse<OrderDto>>.SuccessResponse(response);
        }

        /// <summary>
        /// تحديث حالة الطلب (أدمن)
        /// </summary>
        public async Task<ApiResponse<OrderDto>> UpdateOrderStatusAsync(
            int orderId, UpdateOrderStatusDto dto, int adminId)
        {
            var order = await _unitOfWork.Orders.GetFirstWithIncludeAsync(
                o => o.Id == orderId,
                o => o.User,
                o => o.OrderItems);

            if (order == null)
            {
                throw new NotFoundException("الطلب", orderId);
            }

            // التحقق من صحة الحالة
            if (!Enum.TryParse<OrderStatus>(dto.Status, true, out var newStatus))
            {
                return ApiResponse<OrderDto>.FailureResponse(
                    "حالة الطلب غير صحيحة. الخيارات: Pending, Processing, Shipped, Delivered, Cancelled");
            }

            var oldStatus = order.Status;

            // قواعد تغيير الحالة
            var validTransitions = new Dictionary<OrderStatus, List<OrderStatus>>
            {
                { OrderStatus.Pending, new List<OrderStatus> { OrderStatus.Processing, OrderStatus.Cancelled } },
                { OrderStatus.Processing, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.Cancelled } },
                { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.Delivered } },
                { OrderStatus.Delivered, new List<OrderStatus>() },
                { OrderStatus.Cancelled, new List<OrderStatus>() }
            };

            if (!validTransitions[oldStatus].Contains(newStatus))
            {
                return ApiResponse<OrderDto>.FailureResponse(
                    $"لا يمكن تغيير حالة الطلب من '{oldStatus}' إلى '{newStatus}'");
            }

            order.Status = newStatus;

            // لو تم إلغاء الطلب، ارجع الكمية للمخزون
            if (newStatus == OrderStatus.Cancelled)
            {
                var orderItems = await _unitOfWork.OrderItems.GetAllWithIncludeAsync(
                    oi => oi.OrderId == orderId,
                    oi => oi.Product);

                foreach (var item in orderItems)
                {
                    item.Product.StockQuantity += item.Quantity;
                    _unitOfWork.Products.Update(item.Product);
                }

                // تحديث حالة الدفع
                var payment = await _unitOfWork.Payments.FindAsync(p => p.OrderId == orderId);
                if (payment != null)
                {
                    payment.Status = PaymentStatus.Failed;
                    _unitOfWork.Payments.Update(payment);
                }
            }

            // لو تم التسليم، حدّث حالة الدفع
            if (newStatus == OrderStatus.Delivered)
            {
                var payment = await _unitOfWork.Payments.FindAsync(p => p.OrderId == orderId);
                if (payment != null)
                {
                    payment.Status = PaymentStatus.Completed;
                    _unitOfWork.Payments.Update(payment);
                }
            }

            _unitOfWork.Orders.Update(order);
            await _unitOfWork.SaveChangesAsync();

            // تسجيل العملية
            await _adminLogService.LogActionAsync(
                adminId, "تحديث حالة طلب", "Order", orderId,
                oldValues: oldStatus.ToString(),
                newValues: newStatus.ToString());

            var orderDto = _mapper.Map<OrderDto>(order);
            var items2 = await _unitOfWork.OrderItems.GetAllWithIncludeAsync(
                oi => oi.OrderId == orderId,
                oi => oi.Product);
            orderDto.Items = _mapper.Map<List<OrderItemDto>>(items2);

            return ApiResponse<OrderDto>.SuccessResponse(orderDto, "تم تحديث حالة الطلب بنجاح");
        }

        /// <summary>
        /// إلغاء طلب (المستخدم - قبل الشحن فقط)
        /// </summary>
        public async Task<ApiResponse<bool>> CancelOrderAsync(int orderId, int userId)
        {
            var order = await _unitOfWork.Orders.FindAsync(
                o => o.Id == orderId && o.UserId == userId);

            if (order == null)
            {
                throw new NotFoundException("الطلب", orderId);
            }

            // التحقق من إمكانية الإلغاء
            if (order.Status != OrderStatus.Pending && order.Status != OrderStatus.Processing)
            {
                return ApiResponse<bool>.FailureResponse(
                    "لا يمكن إلغاء الطلب بعد الشحن");
            }

            order.Status = OrderStatus.Cancelled;
            _unitOfWork.Orders.Update(order);

            // إرجاع الكمية للمخزون
            var orderItems = await _unitOfWork.OrderItems.GetAllWithIncludeAsync(
                oi => oi.OrderId == orderId,
                oi => oi.Product);

            foreach (var item in orderItems)
            {
                item.Product.StockQuantity += item.Quantity;
                _unitOfWork.Products.Update(item.Product);
            }

            // تحديث حالة الدفع
            var payment = await _unitOfWork.Payments.FindAsync(p => p.OrderId == orderId);
            if (payment != null)
            {
                payment.Status = PaymentStatus.Failed;
                _unitOfWork.Payments.Update(payment);
            }

            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "تم إلغاء الطلب بنجاح");
        }
    }
}