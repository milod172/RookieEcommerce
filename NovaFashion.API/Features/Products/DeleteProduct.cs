using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using NJsonSchema.Annotations;
using NovaFashion.API.Infrastructure.Persistence;


namespace NovaFashion.API.Features.Products
{

    public record DeleteProductRequest
    {
        [BindFrom("id")]
        [JsonSchemaIgnore]
        public Guid Id { get; set; }
    }

    public class DeleteProduct(AppDbContext db) : Endpoint<DeleteProductRequest>
    {
        public override void Configure()
        {
            Delete("{id}");
            AllowAnonymous();
            Group<ProductGroup>();
        }

        public override async Task HandleAsync(DeleteProductRequest req, CancellationToken ct)
        {
            var product = await db.Products
                .FirstOrDefaultAsync(p => p.Id == req.Id, ct);

            if (product == null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync(ct);

            await Send.OkAsync(null, ct);
        }
    }
}
