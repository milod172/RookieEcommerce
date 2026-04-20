using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace NovaFashion.API.Shared.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(IConfiguration config)
        {
            var account = new Account(
                config["Cloudinary:CloudName"],
                config["Cloudinary:ApiKey"],
                config["Cloudinary:ApiSecret"]);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "products"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            return result.SecureUrl.ToString();
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            // Extract publicId từ Cloudinary URL
            // URL dạng: https://res.cloudinary.com/{cloud}/image/upload/v123456/{publicId}.jpg
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/');

            // Lấy phần sau "upload/" bỏ version (v123456) và extension
            var uploadIndex = Array.IndexOf(segments, "upload");
            var publicIdWithExtension = string.Join("/", segments[(uploadIndex + 2)..]);
            var publicId = Path.ChangeExtension(publicIdWithExtension, null); 

            var deleteParams = new DeletionParams(publicId);
            await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}
