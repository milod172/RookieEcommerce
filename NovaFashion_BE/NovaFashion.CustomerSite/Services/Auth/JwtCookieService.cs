using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using NovaFashion.SharedViewModels.AuthenticationDtos;

namespace NovaFashion.CustomerSite.Services.Auth
{
    public class JwtCookieService(ILogger<JwtCookieService> logger)
    {
        private const string AccessTokenCookie = "nova_at";
        private const string RefreshTokenCookie = "nova_rt";
        private const string UserIdCookie = "nova_uid";


        // Sign In: decode JWT → ClaimsPrincipal → Cookie Authentication
        public async Task SignInAsync(HttpContext context, TokenResponse token)
        {
            var claims = DecodeJwtClaims(token.AccessToken);
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = token.AccessExpiry,
                AllowRefresh = true,
            };

            //  Sign in Cookie Authentication (browser session)
            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties
            );

            // 2. Lưu JWT access token vào HttpOnly cookie để attach vào API calls
            SetSecureCookie(context, AccessTokenCookie, token.AccessToken,
                token.AccessExpiry);

            // 3. Lưu refresh token + userId để auto-refresh sau
            SetSecureCookie(context, RefreshTokenCookie, token.RefreshToken,
                token.RefreshExpiry);
            SetSecureCookie(context, UserIdCookie, token.UserId,
                token.RefreshExpiry);

            logger.LogInformation("User {UserId} signed in successfully", token.UserId);
        }

        
        public async Task SignOutAsync(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            DeleteCookie(context, AccessTokenCookie);
            DeleteCookie(context, RefreshTokenCookie);
            DeleteCookie(context, UserIdCookie);

            logger.LogInformation("User signed out");
        }

       
        // Read JWT access token from cookie when call API
        public string? GetAccessToken(HttpContext context)
            => context.Request.Cookies[AccessTokenCookie];

        public (string? UserId, string? RefreshToken) GetRefreshInfo(HttpContext context)
            => (context.Request.Cookies[UserIdCookie],
                context.Request.Cookies[RefreshTokenCookie]);

        // Check access token expire 
        public bool IsAccessTokenExpired(HttpContext context)
        {
            var token = GetAccessToken(context);
            if (string.IsNullOrEmpty(token)) return true;

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                // Thêm 30s buffer để tránh race condition
                return jwt.ValidTo < DateTime.UtcNow.AddSeconds(30);
            }
            catch
            {
                return true;
            }
        }

        
        // Decode JWT 
        private List<Claim> DecodeJwtClaims(string accessToken)
        {
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var jwt = handler.ReadJwtToken(accessToken);
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, jwt.Claims.First(c => c.Type == "UserId").Value),
                    new(ClaimTypes.Email,          jwt.Claims.First(c => c.Type == "Email").Value),
                    new(ClaimTypes.Role,           jwt.Claims.First(c => c.Type == "role").Value),
                };

                logger.LogDebug("Decoded {Count} claims from JWT", claims.Count);
                return claims;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to decode JWT claims");
                return [];
            }
        }
  
        // Helpers
        private static void SetSecureCookie(
            HttpContext context,
            string name,
            string value,
            DateTime expires)
        {
            context.Response.Cookies.Append(name, value, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,                 // Use for Https
                SameSite = SameSiteMode.Lax,   // Lax cho phép redirect từ external
                Expires = expires,
                IsEssential = true              // GDPR: không bị chặn bởi consent
            });
        }

        private static void DeleteCookie(HttpContext context, string name)
        {
            context.Response.Cookies.Delete(name, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax
            });
        }
    }
}
