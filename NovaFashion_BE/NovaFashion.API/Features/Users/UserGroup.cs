using FastEndpoints;
using FastEndpoints.Swagger;

namespace NovaFashion.API.Features.Users
{
    public class UserGroup : Group
    {
        public UserGroup()
        {
            Configure("users", ep =>
            {
                ep.Description(x => x
                    .ProducesProblemDetails(500)
                    .AutoTagOverride("User")
                    .WithGroupName("User"));
            });
        }
    }
}
