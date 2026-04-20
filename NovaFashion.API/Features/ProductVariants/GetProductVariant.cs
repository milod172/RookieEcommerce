using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Features.Products;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Pagination;
using NovaFashion.API.Shared.Validators;
using NovaFashion.SharedViewModels.ProductVariantDtos;

namespace NovaFashion.API.Features.ProductVariants
{
    public class GetProductVariantMapper : Mapper<PaginationQuery, PaginationList<ProductVariantDto>, PaginationList<ProductVariant>>
    {
        public ProductVariantDto MapToDto(ProductVariant e)
        {
            return new ProductVariantDto
            {
                Id = e.Id,
                ProductName = e.Product?.ProductName ?? string.Empty,
                ProductId = e.ProductId,
                Size = e.Size.ToString(),   
                StockQuantity = e.StockQuantity,
                VariantSku = e.VariantSku,  
                CreatedTime = e.CreatedTime,
                ModifiedTime = e.ModifiedTime ?? e.CreatedTime,
                UnitPrice = e.UnitPrice,
            };
        }

        public override PaginationList<ProductVariantDto> FromEntity(PaginationList<ProductVariant> e)
        {
            var dtos = e.Items.Select(x => MapToDto(x)).ToList();

            return new PaginationList<ProductVariantDto>(
                dtos,
                e.TotalCount,
                e.PageNumber,
                e.PageSize
            );
        }
    }

    public class GetProductVariants(AppDbContext db) : Endpoint<PaginationQuery, PaginationList<ProductVariantDto>, GetProductVariantMapper>
    {
        
        public override void Configure()
        {
            Get("products/{product_id}/variants");
            AllowAnonymous();
            Validator<PaginationQueryValidator>();
            Group<ProductVariantGroup>();
        }

        public override async Task HandleAsync(PaginationQuery req, CancellationToken ct)
        {
            var productId = Route<Guid>("product_id");

            var query = db.ProductVariants
                .AsNoTracking()
                .Where(v => v.ProductId == productId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(req.SortBy))
            {
                query = query.ApplySorting(req.SortBy);
            }
            
            var pageResult = await query.PaginateAsync(req.PageNumber, req.PageSize, ct);
            var response = Map.FromEntity(pageResult);

            await Send.OkAsync(response, ct);
        }
    }
}
