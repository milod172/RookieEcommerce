namespace NovaFashion.API.Configuration
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new();
        public Cloudinary Cloudinary { get; set; } = new();
    }
}
