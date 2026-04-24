using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Pagination;
using NovaFashion.SharedViewModels.CategoryDtos;

namespace NovaFashion.API.Features.Categories
{

    public class GetCategoryMapper : Mapper<PaginationQuery, PaginationList<CategoryDto>, PaginationList<Category>>
    {
        public CategoryDto MapToDto(Category e)
        {
            return new CategoryDto
            {
                Id = e.Id,
                CategoryName = e.CategoryName,
                Description = e.Description,
                ParentCategoryId = e.ParentCategoryId,
                CreatedTime = e.CreatedTime,
                ModifiedTime = e.ModifiedTime
            };
        }

        public override PaginationList<CategoryDto> FromEntity(PaginationList<Category> e)
        {
            var dtos = e.Items.Select(MapToDto).ToList();

            return new PaginationList<CategoryDto>(
                dtos,
                e.TotalCount,
                e.PageNumber,
                e.PageSize
            );
        }
    }

    public class GetCategory(AppDbContext db) : Endpoint<PaginationQuery, PaginationList<CategoryDto>, GetCategoryMapper>
    {
       
        public override void Configure()
        {
            Get("");
            AllowAnonymous();
            Group<CategoryGroup>();
        }

        public override async Task HandleAsync(PaginationQuery req, CancellationToken ct)
        {
            var query = db.Categories
                           .AsNoTracking()
                           .AsQueryable();

           
            if (!string.IsNullOrEmpty(req.SortBy))
            {
                query = query.ApplySorting(req.SortBy);
            }
            
            var pagedEntities = await query.PaginateAsync(req.PageNumber, req.PageSize, ct);

            var response = Map.FromEntity(pagedEntities);

            await Send.OkAsync(response, ct);
        }
    }
}
