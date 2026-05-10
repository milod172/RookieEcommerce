using FastEndpoints;
using FastEndpoints.Swagger;

namespace NovaFashion.API.Features.OrdersTable
{
    public class OrderGroup : Group
    {
        public OrderGroup()
        {
            Configure("orders", ep =>
            {
                ep.Description(x => x
                    .ProducesProblemDetails(500)
                    .AutoTagOverride("Order")
                    .WithGroupName("Order"));
            });
        }
    }
}
