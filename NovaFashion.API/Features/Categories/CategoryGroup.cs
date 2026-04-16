using FastEndpoints;

namespace NovaFashion.API.Features.Categories
{
    public class CategoryGroup : Group
    {
        public CategoryGroup()
        {
            Configure("categories", ep =>
            {
                ep.Description(x => x
                    .WithTags("Category"));
            });
        }
    }
}
