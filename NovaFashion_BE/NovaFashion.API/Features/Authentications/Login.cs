using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;
using NovaFashion.API.Entities;

namespace NovaFashion.API.Features.Authentications
{
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginEndpoint(UserManager<ApplicationUser> userManager) : Endpoint<LoginRequest, TokenResponse>
    {
        public override void Configure()
        {
            Post("/login");
            Group<AuthGroup>();
        }

        public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
        {
            var user = await userManager.FindByEmailAsync(req.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, req.Password))
            {
                ThrowError("Email hoặc mật khẩu không đúng");
                return;
            }

            var roles = await userManager.GetRolesAsync(user);

            var response = await CreateTokenWith<MyTokenService>(user.Id, u =>
            {
                u.Roles.AddRange(roles);
                u.Claims.Add(new("UserId", user.Id));
                u.Claims.Add(new("Email", user.Email!));
            });

            await Send.OkAsync(response, cancellation: ct);
        }
    }
}
