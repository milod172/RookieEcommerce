using FastEndpoints;
using FastEndpoints.Swagger;

namespace NovaFashion.API.Features.ProductVariants
{
    public class ProductVariantGroup : Group
    {
        public ProductVariantGroup()
        {
            Configure("", ep =>
            {
                ep.Description(x => x
                    .ProducesProblemDetails(500)
                    .AutoTagOverride("Product Variant")
                    .WithGroupName("Product Variant"));
            });
        }
    }
}
