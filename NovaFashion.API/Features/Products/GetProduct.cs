using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Pagination;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.API.Features.Products
{
    public class GetProductValidator : Validator<PaginationQuery>
    {
        public GetProductValidator()
        {
            RuleFor(x => x.SortBy).Must(x => x == "Id asc" || x == "Id desc")
                                  .WithMessage("SortBy must be 'Id desc' or 'Id asc'");
        }
    }

    public class GetProduct : Endpoint<PaginationQuery,PaginationList<ProductDto>>
    {
        private readonly AppDbContext _context;

        public GetProduct(AppDbContext context)
        {
            _context = context;
        }

        public override void Configure()
        {
            Get("/api/products");
            AllowAnonymous(); 
            //RequireAuthorization()
            Description(x => x
                .WithName("Get Products")
                .Produces<PaginationList<ProductDto>>(StatusCodes.Status200OK));

        }
        public override async Task HandleAsync(PaginationQuery req, CancellationToken ct)
        {
            var products =  _context.Products
                .AsNoTracking()
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    Description = p.Description,
                    CreatedTime = p.CreatedTime,
                    CreatedBy = p.CreatedBy,
                    ModifiedTime = p.ModifiedTime,
                    ModifiedBy = p.ModifiedBy
                });

            //Sorting
            if (!string.IsNullOrEmpty(req.SortBy))
            {
                products = req.SortBy.ToLower() switch
                {
                    "id desc" => products.OrderByDescending(x => x.Id),
                    "id asc" => products.OrderBy(x => x.Id),
                    _ => products.OrderByDescending(x => x.Id)
                };
            }

            var pageResult = await products.PaginateAsync(
                req.PageNumber, 
                req.PageSize, 
                ct);

            await Send.OkAsync(pageResult, ct);
        }
    }
}
