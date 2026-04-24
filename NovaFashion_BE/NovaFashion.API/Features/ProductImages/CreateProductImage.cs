using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
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

        public CreateProductImagesValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage(ProductIdRequired);

            RuleFor(x => x.Files)
                .NotNull().WithMessage(FilesCannotBeNull)
                .Must(f => f.Count > 0)
                .WithMessage(AtLeastOneImageRequired);

            RuleFor(x => x.AltText)
                .MaximumLength(200)
                .WithMessage(AltTextTooLong)
                .When(x => !string.IsNullOrEmpty(x.AltText));
        }
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
