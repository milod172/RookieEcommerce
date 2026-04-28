using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
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
                    .Select(MapToDto)   // Recursion
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
                           .ApplyStatusFilter(req.Status)
                           .ApplySortFilter(req.SortBy);


            var pagedEntities = await query.PaginateAsync(req.PageNumber, req.PageSize, ct);

            var allCategories = await db.Categories
                                    .AsNoTracking()
                                    .Where(c => c.IsDeleted == false)
                                    .ToListAsync(ct);

            var response = Map.FromEntity(pagedEntities, allCategories);

            await Send.OkAsync(response, ct);
        }
    }

    internal static class GetCategoryQueryExtensions
    {
        internal static IQueryable<Category> ApplyStatusFilter(
            this IQueryable<Category> query,
            FilterStatus status)
            => status switch
            {
                FilterStatus.Active => query.Where(c => !c.IsDeleted && c.ParentCategoryId == null),
                FilterStatus.Inactive => query.Where(c => c.IsDeleted && c.ParentCategoryId == null),
                _ => query.Where(c => c.ParentCategoryId == null)
            };

        internal static IQueryable<Category> ApplySortFilter(
            this IQueryable<Category> query,
            FilterSort sortBy)
            => sortBy switch
            {
                FilterSort.Oldest => query.OrderBy(c => c.CreatedTime),
                FilterSort.Newest => query.OrderByDescending(c => c.CreatedTime),           
                FilterSort.IdAsc => query.OrderBy(c => c.Id),
                FilterSort.IdDesc => query.OrderByDescending(c => c.Id),
                FilterSort.NameAsc => query.OrderBy(c => c.CategoryName),
                FilterSort.NameDesc => query.OrderByDescending(c => c.CategoryName),
                _ => query
            };
    }
}
