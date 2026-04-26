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
        private List<Category> _allCategories = [];
        public CategoryDto MapToDto(Category e)
        {
            return new CategoryDto
            {
                Id = e.Id,
                CategoryName = e.CategoryName,
                Description = e.Description,
                ParentCategoryId = e.ParentCategoryId,
                SubCategories = _allCategories
                    .Where(c => c.ParentCategoryId == e.Id)
                    .Select(MapToDto)   // Đệ quy
                    .ToList(),
                CreatedTime = e.CreatedTime,
                ModifiedTime = e.ModifiedTime
            };
        }

        public PaginationList<CategoryDto> FromEntity(PaginationList<Category> e, List<Category> allCategories)
        {
            _allCategories = allCategories;
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
                           .Where(c => c.ParentCategoryId == null && c.IsDeleted == false)
                           .AsQueryable();

           
            if (!string.IsNullOrEmpty(req.SortBy))
            {
                query = query.ApplySorting(req.SortBy);
            }
            
            var pagedEntities = await query.PaginateAsync(req.PageNumber, req.PageSize, ct);

            var allCategories = await db.Categories
                                    .AsNoTracking()
                                    .Where(c => c.IsDeleted == false)
                                    .ToListAsync(ct);

            var response = Map.FromEntity(pagedEntities, allCategories);

            await Send.OkAsync(response, ct);
        }
    }
}
