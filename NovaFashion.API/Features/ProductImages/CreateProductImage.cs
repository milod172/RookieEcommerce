using FastEndpoints;
using FluentValidation;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
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
        public CreateProductImagesValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId không được để trống");


            RuleFor(x => x.Files)
                .Must(f => f != null && f.Count > 0)
                .WithMessage("Vui lòng chọn ít nhất một hình ảnh");
                

            RuleFor(x => x.AltText)
                .MaximumLength(200)
                .WithMessage("AltText không được vượt quá 200 ký tự")
                .When(x => x.AltText is not null);
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
    public class CreateProductImages(IProductImageRepository repo,ICloudinaryService cloudinary): Endpoint<CreateProductImagesRequest, List<ProductImageDto>, CreateProductImageMapper>
    {
        public override void Configure()
        {
            Post("/{product_id}/upload");
            AllowFileUploads();
            Group<ProductImageGroup>();
        }


        public override async Task HandleAsync(CreateProductImagesRequest req, CancellationToken ct)
        {
            var entities = new List<ProductImage>();

            // Check if product has any existing images in the database
            bool hasAnyImageInProduct = await repo.HasImagesAsync(req.ProductId, ct);

            // if images already exist, take the next SortOrder. If not, start from 0.
            int nextSortOrder = hasAnyImageInProduct
                                ? await repo.GetMaxSortOrderAsync(req.ProductId, ct) + 1
                                : 0;

            foreach (var file in req.Files)
            {
                var imageUrl = await cloudinary.UploadImageAsync(file);

                var entity = new ProductImage
                {
                    ProductId = req.ProductId,
                    ImageUrl = imageUrl,
                    AltText = req.AltText ?? file.FileName,
                    SortOrder = nextSortOrder,            
                    IsPrimary = (nextSortOrder == 0) // if SortOrder = 0 then it must be Primary
                };

                entities.Add(entity);
                nextSortOrder++; 
            }

            await repo.AddRangeAsync(entities, ct);

            var listImagesDto = entities.Select(e => Map.FromEntity(e)).ToList();   
            await Send.CreatedAsync(listImagesDto, ct);
        }
    }
}
