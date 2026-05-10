using FastEndpoints;
using FastEndpoints.Swagger;

namespace NovaFashion.API.Features.VnPay
{
    public class VnPayGroup : Group
    {
        public VnPayGroup()
        {
            Configure("vnpay", ep =>
            {
                ep.Description(x => x
                    .ProducesProblemDetails(500)
                    .AutoTagOverride("VNPay")
                    .WithGroupName("VNPay"));
            });
        }
    }
}
