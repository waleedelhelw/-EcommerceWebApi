namespace EcommerceWepApi.BLL.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string entityName, int id)
            : base($"{entityName} بالمعرّف {id} غير موجود") { }
    }
}