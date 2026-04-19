using FastEndpoints;

namespace NovaFashion.API.Features.Categories
{
    public class DeleteCategory(ICategoryRepository categoryRepository) : EndpointWithoutRequest
    {
        public override void Configure()
        {
            Delete("{id}");
            AllowAnonymous();

            Group<CategoryGroup>();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var entity = await categoryRepository.FindAsync(Route<Guid>("id"), ct);

            if (entity == null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            await categoryRepository.DeleteAsync(entity, ct);

            await Send.OkAsync(null, ct);
        }
    }
}
