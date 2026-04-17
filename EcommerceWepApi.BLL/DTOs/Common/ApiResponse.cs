namespace EcommerceWepApi.BLL.DTOs.Common
{
    /// <summary>
    /// الاستجابة الموحدة لجميع الـ APIs
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        // استجابة ناجحة
        public static ApiResponse<T> SuccessResponse(T data, string message = "تمت العملية بنجاح")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        // استجابة فاشلة
        public static ApiResponse<T> FailureResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }
}