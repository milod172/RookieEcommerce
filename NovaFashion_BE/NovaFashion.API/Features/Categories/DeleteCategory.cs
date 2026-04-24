using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Infrastructure.Persistence;

namespace NovaFashion.API.Features.Categories
{
    public class DeleteCategoryRequest
    {
        [BindFrom("id")]
        [JsonSchemaIgnore]
        public Guid Id { get; set; }
    }
    public class DeleteCategory(AppDbContext db) : Endpoint<DeleteCategoryRequest>
    {
        public override void Configure()
        {
            Delete("{id}");
            AllowAnonymous();
            Group<CategoryGroup>();
        }

        public override async Task HandleAsync(DeleteCategoryRequest req, CancellationToken ct)
        {
            var category = await db.Categories.FirstOrDefaultAsync(x => x.Id == req.Id, ct);

            if (category == null)
            {
                ThrowError("Không tìm thấy danh mục", statusCode: 404);
            }

            db.Categories.Remove(category);
            await db.SaveChangesAsync(ct);

            await Send.OkAsync(null, ct);   
        }
    }
}
