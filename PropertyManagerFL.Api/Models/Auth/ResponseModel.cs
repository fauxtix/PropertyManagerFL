namespace PropertyManagerFL.Api.Models.Auth
{
    public class ResponseModel<T> : ResponseModel where T : class
    {
        public T? Data { get; set; }
    }

    public class ResponseModel
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
    }
}
