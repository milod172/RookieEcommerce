using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.ProductDtos;

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
                UnitPrice = e.UnitPrice,
                Details = e.Details,
                TotalQuantity = e.TotalQuantity,
                Sku = e.Sku,
                CategoryId = e.CategoryId != null ? e.CategoryId.Value : Guid.Empty,
                CategoryName = e.Category != null ? e.Category.CategoryName : string.Empty,
                CreatedTime = e.CreatedTime
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
            var product = await db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == req.Id, ct);

            if (product == null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            await Send.OkAsync(Map.FromEntity(product), ct);
        }
    }
}
