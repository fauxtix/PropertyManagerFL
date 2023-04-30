namespace PropertyManagerFL.Api.Models
{
    /// <summary>
    /// API response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Result { get; set; }
    }
}
