using FastEndpoints;
using FastEndpoints.Swagger;

namespace NovaFashion.API.Features.Authentications
{
    public class AuthGroup : Group
    {
        public AuthGroup()
        {
            Configure("auth", ep =>
            {
                ep.Description(x => x
                    .ProducesProblemDetails(500)
                    .AutoTagOverride("Authentication")
                    .WithGroupName("Authentication"));
            });
        }
    }
}

