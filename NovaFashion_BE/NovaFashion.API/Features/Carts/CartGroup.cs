using FastEndpoints;
using FastEndpoints.Swagger;

namespace NovaFashion.API.Features.Carts
{
    public class CartGroup : Group
    {
        public CartGroup()
        {
            Configure("carts", ep =>
            {
                ep.Description(x => x
                    .ProducesProblemDetails(500)
                    .AutoTagOverride("Cart")
                    .WithGroupName("Cart"));
            });
        }
    }
}
