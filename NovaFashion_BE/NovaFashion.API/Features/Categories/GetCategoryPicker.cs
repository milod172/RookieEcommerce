using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Infrastructure.Persistence;
using NovaFashion.SharedViewModels.CategoryDtos;

namespace NovaFashion.API.Features.Categories
{
    public class GetCategoryPicker(AppDbContext db) : EndpointWithoutRequest<List<CategoryPickerDto>>
    {
        public override void Configure()
        {
            Get("picker");
            Group<CategoryGroup>();
        }
        public override async Task HandleAsync(CancellationToken ct)
        {
            var lookups = await db.Categories
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .Select(c => new CategoryPickerDto
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    Description = c.Description,
                    ParentCategoryId = c.ParentCategoryId
                })
                .ToListAsync(ct);

            await Send.OkAsync(lookups, ct);
        }
    }
}
