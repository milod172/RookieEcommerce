using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Pagination;
using NovaFashion.API.Shared.Validators;
using NovaFashion.SharedViewModels.CategoryDtos;

namespace NovaFashion.API.Features.Categories
{
    public class GetCategory : Endpoint<PaginationQuery, PaginationList<CategoryDto>>
    {
        private readonly AppDbContext _context;
        public GetCategory(AppDbContext context)
        {
            _context = context;
        }
        public override void Configure()
        {
            Get("");
            Group<CategoryGroup>();
            AllowAnonymous();
            //RequireAuthorization()
            Validator<PaginationQueryValidator>();
            Description(x => x
                .WithName("GetCategories")
                .Produces<PaginationList<CategoryDto>>(StatusCodes.Status200OK));
        }
    
        public override async Task HandleAsync(PaginationQuery req, CancellationToken ct)
        {
            var query = _context.Categories
            .AsNoTracking()
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                Description = c.Description,
                ParentCategoryId = c.ParentCategoryId
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
