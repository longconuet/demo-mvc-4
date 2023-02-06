namespace WebApplication4.ModalViews
{
    public class ServiceResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }
    }

    public class ServiceResponse
    {
        public int Status { get; set; }
        public string Message { get; set; } = "";
    }
}
