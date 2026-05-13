using FastEndpoints;
using FastEndpoints.Swagger;

namespace NovaFashion.API.Features.ProductRatings
{
    public class ProductRatingGroup : Group
    {
        public ProductRatingGroup()
        {
            Configure("product-rating", ep =>
            {
                ep.Description(x => x
                    .ProducesProblemDetails(500)
                    .AutoTagOverride("Product Rating"));
            });
        }
    }
}
