using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Pagination;
using NovaFashion.API.Shared.Validators;
using NovaFashion.SharedViewModels.ProductDtos;
using NovaFashion.SharedViewModels.ProductImageDtos;

namespace NovaFashion.API.Features.Products
{
    public class GetProductMapper : Mapper<PaginationQuery,PaginationList<ProductDto>, PaginationList<Product>>
    {
        public ProductDto MapToDto(Product e)
        {
            return new ProductDto
            { 
                Id = e.Id,
                ProductName = e.ProductName,
                Description = e.Description,
                UnitPrice = e.UnitPrice,
                Images = e.ProductImages
                    .OrderBy(pi => pi.SortOrder)
                    .Select(pi => new ProductImageInProductDto
                    {
                        ImageUrl = pi.ImageUrl,
                        AltText  = pi.AltText ?? string.Empty,
                        SortOrder = pi.SortOrder,
                        IsPrimary = pi.IsPrimary
                    })
                    .ToList()
            };
        }

        public override PaginationList<ProductDto> FromEntity(PaginationList<Product> e)
        {
            var dtos = e.Items.Select(x => MapToDto(x)).ToList();

            return new PaginationList<ProductDto>(
                dtos,
                e.TotalCount,
                e.PageNumber,
                e.PageSize
            );
        }
    }

    public class GetProduct(AppDbContext db) : Endpoint<PaginationQuery, PaginationList<ProductDto>, GetProductMapper>
    {
        public override void Configure()
        {
            Get("");
            AllowAnonymous();
            Validator<PaginationQueryValidator>();
            Group<ProductGroup>();
        }

        public override async Task HandleAsync(PaginationQuery req, CancellationToken ct)
        {
            var query = db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .AsQueryable();

            if (!string.IsNullOrEmpty(req.SortBy))
            {
                query = query.ApplySorting(req.SortBy);
            }

            var pageResultEntities = await query.PaginateAsync(req.PageNumber, req.PageSize, ct);
            var pageResultDtos = Map.FromEntity(pageResultEntities);

            await Send.OkAsync(pageResultDtos, ct);
        }
    }
}
