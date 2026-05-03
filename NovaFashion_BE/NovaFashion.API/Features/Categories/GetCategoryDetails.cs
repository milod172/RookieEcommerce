using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.CategoryDtos;

namespace NovaFashion.API.Features.Categories
{
    public class GetCategoryDetailsQuery
    {
        [BindFrom("id")]
        [JsonSchemaIgnore]
        public Guid Id { get; set; }
    }

    public class GetCategoryDetailsMapper : Mapper<GetCategoryDetailsQuery, CategoryDetailsDto, Category>
    {
        public override CategoryDetailsDto FromEntity(Category e)
        {
            return new CategoryDetailsDto
            {
                Id = e.Id,
                CategoryName = e.CategoryName,
                Description = e.Description,
                ParentCategoryName = e.ParentCategory != null ? e.ParentCategory.CategoryName : string.Empty,
                ParentCategoryId = e.ParentCategoryId,
                IsDeleted = e.IsDeleted
            };
        }
    }

    public class GetCategoryDetails(AppDbContext db) : Endpoint<GetCategoryDetailsQuery, CategoryDetailsDto, GetCategoryDetailsMapper>
    {
        public override void Configure()
        {
            Get("{id}");
            Group<CategoryGroup>();
        }
        public override async Task HandleAsync(GetCategoryDetailsQuery req, CancellationToken ct)
        {
            var category = await db.Categories
                .AsNoTracking()
                .Include(c => c.ParentCategory)
                .FirstOrDefaultAsync(p => p.Id == req.Id, ct);

            if (category == null)
            {
                ThrowError("Không tìm thấy danh mục", statusCode: 404);
                return;
            }

            await Send.OkAsync(Map.FromEntity(category), ct);
        }
    }
}
