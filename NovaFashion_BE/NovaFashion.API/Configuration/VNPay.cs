namespace NovaFashion.API.Configuration
{
    public class VNPay
    {
        public string TmnCode { get; set; } = string.Empty;
        public string HashSecret { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
    }
}
