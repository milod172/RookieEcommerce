using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.ProductDtos;
using NovaFashion.SharedViewModels.ProductImageDtos;
using NovaFashion.SharedViewModels.ProductVariantDtos;

namespace NovaFashion.API.Features.Products
{
    public class GetProductDetailsRequest
    {
        [BindFrom("id")]
        [JsonSchemaIgnore]
        public Guid Id { get; set; }
    }

    public class GetProductDetailsMapper : Mapper<GetProductDetailsRequest, ProductDetailsDto, Product>
    {
        public override ProductDetailsDto FromEntity(Product e) 
        {
            return new ProductDetailsDto {
                Id = e.Id,
                ProductName = e.ProductName,
                Description = e.Description,
                UnitPrice = e.ProductVariants.Any() 
                    ? e.ProductVariants.Min(v => v.UnitPrice) 
                    : e.UnitPrice,
                Details = e.Details,
                TotalQuantity = e.TotalQuantity,
                TotalSell = e.TotalSell,
                Sku = e.Sku,
                CategoryId = e.CategoryId != null ? e.CategoryId.Value : Guid.Empty,
                CategoryName = e.Category != null ? e.Category.CategoryName : string.Empty,
                Images = e.ProductImages
                    .OrderBy(pi => pi.SortOrder)
                    .Select(pi => new ProductImageInProductDto
                    {
                        Id = pi.Id,
                        ImageUrl = pi.ImageUrl,
                        AltText = pi.AltText ?? string.Empty,
                        SortOrder = pi.SortOrder,
                        IsPrimary = pi.IsPrimary
                    })
                    .ToList(),
                Variants = e.ProductVariants
                    .OrderBy(pv => pv.Size)
                    .Select(pv => new ProductVariantInProductDto
                    {
                        Id = pv.Id,
                        Size = pv.Size.ToString(),
                        StockQuantity = pv.StockQuantity,
                        IsAvailable = pv.StockQuantity > 0,
                        IsExceedStock = pv.Product.TotalQuantity > pv.StockQuantity,
                        UnitPrice = pv.UnitPrice,
                        VariantSku = pv.VariantSku
                    })
                    .ToList(),
                CreatedTime = e.CreatedTime,
                IsDeleted = e.IsDeleted,
            };
        }

    }
    public class GetProductDetails(AppDbContext db) : Endpoint<GetProductDetailsRequest, ProductDetailsDto, GetProductDetailsMapper>
    {
        public override void Configure()
        {
            Get("{id}");
            AllowAnonymous();
            Group<ProductGroup>();
        }

        public override async Task HandleAsync(GetProductDetailsRequest req, CancellationToken ct)
        {
            var isAdmin = User.IsInRole(Role.Admin.ToString());

            var product = await db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants.Where(v => isAdmin || !v.IsDeleted)) //if is Admin get all, not get Deleted=false
                .FirstOrDefaultAsync(p => p.Id == req.Id, ct);

            if (product == null)
            {
                ThrowError("Không tìm thấy sản phẩm", statusCode: 404);
                return;
            }

            await Send.OkAsync(Map.FromEntity(product), ct);
        }
    }
}
