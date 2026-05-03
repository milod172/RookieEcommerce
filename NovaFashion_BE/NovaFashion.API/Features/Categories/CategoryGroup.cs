using FastEndpoints;
using FastEndpoints.Swagger;

namespace NovaFashion.API.Features.Categories
{
    public class CategoryGroup : Group
    {
        public CategoryGroup()
        {
            Configure("categories", ep =>
            {
                ep.Description(x => x
                    .ProducesProblemDetails(500)
                    .AutoTagOverride("Category")
                    .WithGroupName("Category"));
            });
        }
    }
}
