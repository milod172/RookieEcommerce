using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Services;

namespace NovaFashion.API.Features.ProductImages
{
    public class DeleteProductImageRequest
    {
        [BindFrom("product_id")]
        [JsonSchemaIgnore]
        public Guid ProductId { get; set; }

        [BindFrom("image_id")]
        [JsonSchemaIgnore]
        public Guid ImageId { get; set; }
    }

    public class DeleteProductImageValidator : Validator<DeleteProductImageRequest>
    {
        public const string ProductIdRequired = "ProductId không được để trống";
        public const string ImageIdRequired = "ImageId không được để trống";

        public DeleteProductImageValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage(ProductIdRequired);

            RuleFor(x => x.ImageId)
                .NotEmpty()
                .WithMessage(ImageIdRequired);
        }
    }

    public class DeleteProductImage(AppDbContext db, ICloudinaryService cloudinary) : Endpoint<DeleteProductImageRequest>
    {

        public override void Configure()
        {
            Delete("products/{product_id}/images/{image_id}");
            Group<ProductImageGroup>();
        }

        public override async Task HandleAsync(DeleteProductImageRequest req, CancellationToken ct)
        {
            var imageUrl = await DeleteImageAndReorderAsync(req.ImageId, req.ProductId, ct);

            if (imageUrl is null)
            {
                ThrowError("Không tìm thấy hình ảnh", statusCode: 404);
            }

            // Delete from Cloudinary
            await cloudinary.DeleteImageAsync(imageUrl);

            await Send.OkAsync("Delete images successfully", ct);
        }

        #region Delete Image and Reorder
        private async Task<string?> DeleteImageAndReorderAsync(Guid imageId, Guid productId, CancellationToken ct)
        {
            var image = await db.ProductImages
                .FirstOrDefaultAsync(x => x.Id == imageId, ct);

            if (image is null)
                return null;

            var imageUrl = image.ImageUrl;

            db.ProductImages.Remove(image);
            await db.SaveChangesAsync(ct);

            // Reorder remaining images
            await ReorderImagesInternalAsync(productId, ct);

            return imageUrl;
        }

        private async Task ReorderImagesInternalAsync(Guid productId, CancellationToken ct)
        {
            var remainingImages = await db.ProductImages
                .Where(x => x.ProductId == productId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync(ct);

            for (int i = 0; i < remainingImages.Count; i++)
            {
                remainingImages[i].SortOrder = i;
                remainingImages[i].IsPrimary = (i == 0);
            }

            await db.SaveChangesAsync(ct);
        }

        #endregion
    }
}
