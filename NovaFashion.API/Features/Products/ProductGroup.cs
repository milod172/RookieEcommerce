using FastEndpoints;

namespace NovaFashion.API.Features.Products
{
    public class ProductGroup : Group
    {
        public ProductGroup()
        {
            Configure("products", ep =>
            {
                ep.Description(x => x
                    .WithTags("Product"));
            });
        }
    }
}
