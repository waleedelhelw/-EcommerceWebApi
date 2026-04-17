namespace EcommerceWepApi.BLL.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "ليس لديك صلاحية لهذه العملية") : base(message) { }
    }
}