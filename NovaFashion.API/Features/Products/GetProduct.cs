using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Pagination;
using NovaFashion.API.Shared.Validators;
using NovaFashion.SharedViewModels.ProductDtos;

namespace NovaFashion.API.Features.Products
{
    public class GetProduct : Endpoint<PaginationQuery,PaginationList<ProductDto>>
    {
        private readonly AppDbContext _context;
        public GetProduct(AppDbContext context)
        {
            _context = context;
        }
        public override void Configure()
        {
            Get("");
            Group<ProductGroup>();
            AllowAnonymous(); 
            //RequireAuthorization()
            Validator<PaginationQueryValidator>();
            Description(x => x
                .WithName("GetProducts")
                .Produces<PaginationList<ProductDto>>(StatusCodes.Status200OK));

        }
        public override async Task HandleAsync(PaginationQuery req, CancellationToken ct)
        {
            var query =  _context.Products
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

          
            if (!string.IsNullOrEmpty(req.SortBy))
            {
                query = query.ApplySorting(req.SortBy);
            }

            var pageResult = await query.PaginateAsync(
                req.PageNumber, 
                req.PageSize, 
                ct);

            await Send.OkAsync(pageResult, ct);
        }
    }
}
