namespace NovaFashion.CustomerSite.Configuration
{
    public class AppSettings
    {
        public JwtSettings JwtSettings { get; set; } = new();
        public ApiSettings ApiSettings { get; set; } = new();
    }
}
