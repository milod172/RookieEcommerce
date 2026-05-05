using FastEndpoints;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Pagination;
using NovaFashion.API.Shared.Validators;
using NovaFashion.SharedViewModels.ProductDtos;
using NovaFashion.SharedViewModels.ProductImageDtos;

namespace NovaFashion.API.Features.Products
{
    public class PaginationProductQuery : PaginationQuery
    {
        [QueryParam]
        public decimal? MinPrice { get; set; }

        [QueryParam]
        public decimal? MaxPrice { get; set; }

        [QueryParam]
        public Guid? CategoryId { get; set; }
    }

    public class GetProductMapper : Mapper<PaginationProductQuery, PaginationList<ProductDto>, PaginationList<Product>>
    {
        public ProductDto MapToDto(Product e)
        {
            return new ProductDto
            { 
                Id = e.Id,
                ProductName = e.ProductName,
                Description = e.Description,
                UnitPrice = e.ProductVariants.Any()
                    ? e.ProductVariants.Min(v => v.UnitPrice)
                    : e.UnitPrice,     
                Sku = e.Sku,
                TotalQuantity = e.TotalQuantity,
                TotalSell = e.TotalSell,
                IsDeleted = e.IsDeleted,
                Images = e.ProductImages
                    .OrderBy(pi => pi.SortOrder)
                    .Select(pi => new ProductImageInProductDto
                    {
                        Id = pi.Id,
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

    public class GetProduct(AppDbContext db) : Endpoint<PaginationProductQuery, PaginationList<ProductDto>, GetProductMapper>
    {
        public override void Configure()
        {
            Get("");
            AllowAnonymous();
            Validator<PaginationQueryValidator>();
            Group<ProductGroup>();
        }

        public override async Task HandleAsync(PaginationProductQuery req, CancellationToken ct)
        {
            if(req.MinPrice != null || req.MaxPrice != null)
            {
                if(req.MinPrice == 0 && req.MaxPrice == 0)
                {
                    AddError(new ValidationFailure("price_filter_error", "Vui lòng điền khoảng giá phù hợp"));
                }
                else if (req.MinPrice > req.MaxPrice) { 
                    AddError(new ValidationFailure("price_filter_error", "Vui lòng điền khoảng giá phù hợp")); 
                }
            }

            ThrowIfAnyErrors();

            var query = db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductVariants)
                .ApplyStatusFilter(req.Status)
                .ApplySortFilter(req.SortBy)
                .ApplyPriceFilter(req.MinPrice, req.MaxPrice)  
                .ApplyCategoryFilter(req.CategoryId);


            var pageResultEntities = await query.PaginateAsync(req.PageNumber, req.PageSize, ct);
            var pageResultDtos = Map.FromEntity(pageResultEntities);

            await Send.OkAsync(pageResultDtos, ct);
        }
    }

    internal static class GetProductQueryExtensions
    {
        internal static IQueryable<Product> ApplyStatusFilter(
            this IQueryable<Product> query,
            FilterStatus status)
            => status switch
            {
                FilterStatus.Active => query.Where(p => !p.IsDeleted),
                FilterStatus.Inactive => query.Where(p => p.IsDeleted),
                _ => query
            };

        internal static IQueryable<Product> ApplySortFilter(
            this IQueryable<Product> query,
            FilterSort sortBy)
            => sortBy switch
            {
                FilterSort.Oldest => query.OrderBy(p => p.CreatedTime),
                FilterSort.Newest => query.OrderByDescending(p => p.CreatedTime),
                FilterSort.IdAsc => query.OrderBy(p => p.Id),
                FilterSort.IdDesc => query.OrderByDescending(p => p.Id),
                FilterSort.NameAsc => query.OrderBy(p => p.ProductName),
                FilterSort.NameDesc => query.OrderByDescending(p => p.ProductName),
                _ => query
            };

        internal static IQueryable<Product> ApplyPriceFilter(
            this IQueryable<Product> query,
            decimal? minPrice,
            decimal? maxPrice)
        {
            if (minPrice.HasValue)
                query = query.Where(p =>
                     p.ProductVariants.Any()
                         ? p.ProductVariants.Min(v => v.UnitPrice) >= minPrice.Value
                         : p.UnitPrice >= minPrice.Value
                 );

            if (maxPrice.HasValue)
                query = query.Where(p =>
                    p.ProductVariants.Any()
                        ? p.ProductVariants.Max(v => v.UnitPrice) <= maxPrice.Value
                        : p.UnitPrice <= maxPrice.Value
                );

            return query;
        }

        internal static IQueryable<Product> ApplyCategoryFilter(
            this IQueryable<Product> query,
            Guid? categoryId)
        {
            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            return query;
        }
    }
}
