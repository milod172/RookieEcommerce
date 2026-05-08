using System.Security.Claims;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;

namespace NovaFashion.API.Features.Authentications
{
    public class RevokeRefreshToken(UserManager<ApplicationUser> userManager, AppDbContext db) : EndpointWithoutRequest<string>
    {
        public override void Configure()
        {
            Post("/revoke-rt");
            Group<AuthGroup>();
           
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Type: {claim.Type} | Value: {claim.Value}");
            }

            var userId = User.FindFirstValue("sub");
            var user = db.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                ThrowError("Người dùng không tồn tại"); return;
            }

            await userManager.RemoveAuthenticationTokenAsync(
                user,
                loginProvider: "FastEndpoints",
                tokenName: "RefreshToken");

            await Send.OkAsync("Thu hồi refresh token thành công", cancellation: ct);
        }
    }
}
