namespace NovaFashion.API.Shared.Services
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task DeleteImageAsync(string imageUrl);
    }
}
