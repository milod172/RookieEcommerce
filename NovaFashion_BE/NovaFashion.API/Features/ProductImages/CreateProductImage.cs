using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Services;
using NovaFashion.SharedViewModels.ProductImageDtos;

namespace NovaFashion.API.Features.ProductImages
{
    
    public class CreateProductImagesRequest
    {
       [BindFrom("product_id")]
       [JsonSchemaIgnore]  
        public Guid ProductId { get; set; }   
        public List<IFormFile> Files { get; set; } = [];
        [JsonSchemaIgnore]
        public string? AltText { get; set; }
    }

    public class CreateProductImagesValidator : Validator<CreateProductImagesRequest>
    {
        public const string ProductIdRequired = "ProductId không được để trống";
        public const string AtLeastOneImageRequired = "Vui lòng chọn ít nhất một hình ảnh";
        public const string AltTextTooLong = "AltText không được vượt quá 200 ký tự";
        public const string FilesCannotBeNull = "Danh sách file không được để trống";

        public const string InvalidImageFormat = "File '{0}' không phải định dạng ảnh hợp lệ (jpg, jpeg, png, webp, gif)";
        public const string FileTooLarge = "File '{0}' vượt quá dung lượng cho phép (tối đa 5MB)";
        private static readonly HashSet<string> AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
        private static readonly HashSet<string> AllowedMimeTypes = ["image/jpeg", "image/png", "image/webp", "image/gif"];
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5MB
        public CreateProductImagesValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage(ProductIdRequired);

            RuleFor(x => x.Files)
                .NotNull().WithMessage(FilesCannotBeNull)
                .Must(f => f.Count > 0)
                .WithMessage(AtLeastOneImageRequired);

            RuleForEach(x => x.Files)
            .ChildRules(file =>
            {
                file.RuleFor(f => f)
                    .Must(f => f.Length <= MaxFileSizeBytes)
                    .WithMessage(f => string.Format(FileTooLarge, f.FileName));

                file.RuleFor(f => f)
                    .Must(IsValidImage)
                    .WithMessage(f => string.Format(InvalidImageFormat, f.FileName));
            })
            .When(x => x.Files is { Count: > 0 });

            RuleFor(x => x.AltText)
                .MaximumLength(200)
                .WithMessage(AltTextTooLong)
                .When(x => !string.IsNullOrEmpty(x.AltText));
        }

        #region Validate image input
        private static bool IsValidImage(IFormFile file)
        {
            // 1. Kiểm tra extension
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext)) return false;

            // 2. Kiểm tra MIME type
            if (!AllowedMimeTypes.Contains(file.ContentType.ToLowerInvariant())) return false;

            // 3. Kiểm tra magic bytes (file signature)
            return HasValidImageSignature(file);
        }

        private static bool HasValidImageSignature(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            Span<byte> header = stackalloc byte[12];
            if (stream.Read(header) < 4) return false;

            // JPEG: FF D8 FF
            if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
                return true;

            // PNG: 89 50 4E 47
            if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47)
                return true;

            // GIF: 47 49 46 38
            if (header[0] == 0x47 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x38)
                return true;

            // WebP: 52 49 46 46 ... 57 45 42 50
            if (header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
                header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50)
                return true;

            return false;
        }
        #endregion
    }

    public class CreateProductImageMapper : Mapper<CreateProductImagesRequest, List<ProductImageDto>, ProductImage>
    {
       
        public override List<ProductImageDto> FromEntity(ProductImage e)
        {
            return new List<ProductImageDto> 
            {
                new ProductImageDto
                {
                    Id = e.Id,
                    ImageUrl = e.ImageUrl,
                    AltText = e.AltText,
                    SortOrder = e.SortOrder,
                    IsPrimary = e.IsPrimary,
                    ProductId = e.ProductId
                }
            };
        }
    }
    public class CreateProductImages(AppDbContext db, ICloudinaryService cloudinary) : Endpoint<CreateProductImagesRequest, List<ProductImageDto>, CreateProductImageMapper>
    {
        
        public override void Configure()
        {
            Post("products/{product_id}/images");
            AllowFileUploads();
            Roles(Role.Admin.ToString());
            Group<ProductImageGroup>();
        }

        public override async Task HandleAsync(CreateProductImagesRequest req, CancellationToken ct)
        {
            // Get next SortOrder (if no images → start from 0)
            int nextSortOrder = await GetNextSortOrderAsync(req.ProductId, ct);

            var entities = new List<ProductImage>();

            foreach (var file in req.Files)
            {
                var imageUrl = await cloudinary.UploadImageAsync(file);

                var entity = new ProductImage
                {
                    ProductId = req.ProductId,
                    ImageUrl = imageUrl,
                    AltText = req.AltText ?? file.FileName,
                    SortOrder = nextSortOrder,
                    IsPrimary = (nextSortOrder == 0)
                };

                entities.Add(entity);
                nextSortOrder++;
            }

            db.ProductImages.AddRange(entities);
            await db.SaveChangesAsync(ct);

            var listImagesDto = entities.Select(e => Map.FromEntity(e)).ToList();
            await Send.CreatedAsync(listImagesDto, ct);
        }

        
        #region Get Next SortOrder
        private async Task<int> GetNextSortOrderAsync(Guid productId, CancellationToken ct)
        {
            var maxOrder = await db.ProductImages
                .Where(x => x.ProductId == productId)
                .MaxAsync(x => (int?)x.SortOrder, ct);

            return maxOrder.HasValue ? maxOrder.Value + 1 : 0;
        }
        #endregion

        
    }
}
