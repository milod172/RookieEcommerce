using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Pagination;
using NovaFashion.API.Shared.Validators;
using NovaFashion.SharedViewModels.ProductVariantDtos;

namespace NovaFashion.API.Features.ProductVariants
{
    public class GetVariantsPaginationQuery : PaginationQuery
    {
        [BindFrom("product_id")]
        public Guid ProductId { get; set; } = Guid.Empty;
    }

    public class GetProductVariantMapper : Mapper<GetVariantsPaginationQuery, PaginationList<ProductVariantDto>, PaginationList<ProductVariant>>
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
                ModifiedTime = e.ModifiedTime,
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

    public class GetProductVariants(AppDbContext db) : Endpoint<GetVariantsPaginationQuery, PaginationList<ProductVariantDto>, GetProductVariantMapper>
    {
        
        public override void Configure()
        {
            Get("products/{product_id}/variants");
            AllowAnonymous();
            Validator<PaginationQueryValidator>();
            Group<ProductVariantGroup>();
        }

        public override async Task HandleAsync(GetVariantsPaginationQuery req, CancellationToken ct)
        {
            var product = await db.Products.AnyAsync(p => p.Id == req.ProductId, ct);

            if (!product)
            {
                 ThrowError("Không tìm thấy sản phẩm", statusCode: 404);
            }

            var query = db.ProductVariants
                .Include(v => v.Product)
                .AsNoTracking()
                .Where(v => v.ProductId == req.ProductId)
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
