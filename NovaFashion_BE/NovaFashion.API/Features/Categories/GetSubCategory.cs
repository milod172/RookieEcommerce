using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.CategoryDtos;

namespace NovaFashion.API.Features.Categories
{
    public class GetSubCategoryQuery
    {
        [BindFrom("parent_id")]
        [JsonSchemaIgnore]
        public Guid ParentId { get; set; }
    }
    public class GetSubCategoryMapper : Mapper<GetSubCategoryQuery, List<CategoryDto>, List<Category>>
    {
        public CategoryDto MapToDto(Category e)
        {
            return new CategoryDto
            {
                Id = e.Id,
                CategoryName = e.CategoryName,
                Description = e.Description,
                HasChildren = e.SubCategories.Any(),
                SubCount = e.SubCategories.Count(),
                ParentCategoryId = e.ParentCategoryId,
                IsDeleted = e.IsDeleted,
                CreatedTime = e.CreatedTime,
                ModifiedTime = e.ModifiedTime
            };
        }

        public override List<CategoryDto> FromEntity(List<Category> e)
        {
            return e.Select(MapToDto).ToList();
        }
    }

    public class GetSubCategory(AppDbContext db) : Endpoint<GetSubCategoryQuery, List<CategoryDto>, GetSubCategoryMapper>
    {
        public override void Configure()
        {
            Get("parent/{parent_id}");
            AllowAnonymous();
            Group<CategoryGroup>();
        }

        public override async Task HandleAsync(GetSubCategoryQuery req, CancellationToken ct)
        {
            var parentExists = await db.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Id == req.ParentId && c.IsDeleted == false, ct);

            if (!parentExists)
            {
                ThrowError("Không tìm thấy danh mục cha", statusCode: 404);
            }

            var categories = await db.Categories
                .AsNoTracking()
                .Where(c => c.ParentCategoryId == req.ParentId)
                .Include(c => c.SubCategories)
                .ToListAsync(ct);

            var response = Map.FromEntity(categories);

            await Send.OkAsync(response, ct);
        }
    }
}
