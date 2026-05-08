using System.IdentityModel.Tokens.Jwt;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NovaFashion.API.Configuration;
using NovaFashion.API.Entities;
using NovaFashion.API.Infrastructure.Persistence;

namespace NovaFashion.API.Features.Authentications
{
    public class MyTokenService : RefreshTokenService<TokenRequest, TokenResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;

        public MyTokenService(
            UserManager<ApplicationUser> userManager,
            AppDbContext db,
            JwtSettings jwtSettings)  
        {
            _userManager = userManager;
            _db = db;

            Setup(o =>
            {
                o.TokenSigningKey = jwtSettings.SecretKey;
                o.AccessTokenValidity = TimeSpan.FromMinutes(jwtSettings.TokenExpiryInMinutes);
                o.RefreshTokenValidity = TimeSpan.FromDays(7);
                o.Endpoint("/refresh-token", ep =>
                {
                    ep.Group<AuthGroup>();
                });
            });
        }

        public override async Task PersistTokenAsync(TokenResponse response)
        {
            var user = await _userManager.FindByIdAsync(response.UserId);
            if (user == null) return;

            var tokenRecord = await _db.UserTokens.FirstOrDefaultAsync(t =>
                t.UserId == response.UserId &&
                t.LoginProvider == "FastEndpoints" &&
                t.Name == "RefreshToken");

            if (tokenRecord == null)
            {
                _db.UserTokens.Add(new UserRefreshToken
                {
                    UserId = response.UserId,
                    LoginProvider = "FastEndpoints",
                    Name = "RefreshToken",
                    Value = response.RefreshToken,
                    ExpiresAt = response.RefreshExpiry
                });
            }
            else
            {
                tokenRecord.Value = response.RefreshToken;
                tokenRecord.ExpiresAt = response.RefreshExpiry;
            }

            await _db.SaveChangesAsync();
        }

        public override async Task RefreshRequestValidationAsync(TokenRequest req)
        {
            var user = await _userManager.FindByIdAsync(req.UserId);
            if (user == null)
            {
                AddError("User không tồn tại");
                return;
            }

            var tokenRecord = await _db.UserTokens.FirstOrDefaultAsync(t =>
                t.UserId == req.UserId &&
                t.LoginProvider == "FastEndpoints" &&
                t.Name == "RefreshToken");

            if (tokenRecord == null || tokenRecord.Value != req.RefreshToken)
                AddError(r => r.RefreshToken, "Refresh token không hợp lệ");
            else if (tokenRecord.ExpiresAt < DateTime.UtcNow)
                AddError(r => r.RefreshToken, "Refresh token đã hết hạn");
        }

        public override async Task SetRenewalPrivilegesAsync(TokenRequest req, UserPrivileges privileges)
        {
            var user = await _userManager.FindByIdAsync(req.UserId);
            if (user == null) return;

            var roles = await _userManager.GetRolesAsync(user);
            privileges.Roles.AddRange(roles);
            privileges.Claims.Add(new(JwtRegisteredClaimNames.Sub, user.Id));
        }
    }
}
