namespace NovaFashion.API.Configuration
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new();
        public Cloudinary Cloudinary { get; set; } = new();
        public JwtSettings JwtSettings { get; set; } = new();
        public AdminSettings AdminSettings { get; set; } = new();
    }
}
