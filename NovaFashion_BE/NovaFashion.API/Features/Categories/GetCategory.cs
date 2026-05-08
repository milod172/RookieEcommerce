using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.API.Shared.Extensions;
using NovaFashion.API.Shared.Pagination;
using NovaFashion.SharedViewModels.CategoryDtos;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace NovaFashion.API.Features.Categories
{

    public class GetCategoryMapper : Mapper<PaginationQuery, PaginationList<CategoryDto>, PaginationList<Category>>
    {
        public CategoryDto MapToDto(Category e) => new()
        {
            Id = e.Id,
            CategoryName = e.CategoryName,
            Description = e.Description,
            ParentCategoryId = e.ParentCategoryId,
            IsDeleted = e.IsDeleted,
            CreatedTime = e.CreatedTime,
            ModifiedTime = e.ModifiedTime
        };

        public PaginationList<CategoryDto> FromEntity(PaginationList<Category> e, List<Category> allCategories)
        {
            // 1. Map toàn bộ các item 
            var allDtos = allCategories.Select(MapToDto).ToList();
            var lookup = allDtos.ToDictionary(d => d.Id);

            // 2. Xây dựng cây từ flat list
            foreach (var dto in allDtos)
            {
                if (dto.ParentCategoryId.HasValue && lookup.TryGetValue(dto.ParentCategoryId.Value, out var parent))
                {
                    parent.SubCategories.Add(dto);
                }
            }

            // 3. Chỉ lấy những Root DTOs (là những thằng nằm trong trang hiện tại)
            var rootIds = e.Items.Select(x => x.Id).ToHashSet();
            var pagedDtos = allDtos.Where(d => rootIds.Contains(d.Id)).ToList();

            // 4. Update các field thống kê 
            foreach (var d in allDtos)
            {
                d.HasChildren = d.SubCategories.Any();
                d.SubCount = d.SubCategories.Count;
            }

            return new PaginationList<CategoryDto>(pagedDtos, e.TotalCount, e.PageNumber, e.PageSize);
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
                   .ApplyStatusFilter(req.Status)
                   .ApplySortFilter(req.SortBy);

            var pagedEntities = await query.PaginateAsync(req.PageNumber, req.PageSize, ct);

            if (!pagedEntities.Items.Any())
            {
                await Send.OkAsync(new PaginationList<CategoryDto>([], 0, req.PageNumber, req.PageSize), ct);
                return;
            }

           
            var rootIds = pagedEntities.Items.Select(x => x.Id).ToList();
            var allCategories = await db.Categories
                                        .AsNoTracking() 
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
        {

            query = query.Where(c => c.ParentCategoryId == null);

            return status switch
            {
                FilterStatus.Active => query.Where(c => !c.IsDeleted),
                FilterStatus.Inactive => query.Where(c => c.IsDeleted),
                _ => query
            };
        }

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
