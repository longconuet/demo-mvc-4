namespace WebApplication4.ModelViews
{
    public class ResponseModel<T>
    {
        public int Status { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }
    }

    public class ResponseModel
    {
        public int Status { get; set; }
        public string Message { get; set; } = "";
    }
}
