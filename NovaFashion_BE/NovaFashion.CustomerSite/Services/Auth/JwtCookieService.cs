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


        // Sign In: decode JWT → ClaimsPrincipal → Cookie Authentication
        public async Task SignInAsync(HttpContext context, TokenResponse token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token.AccessToken);

            var identity = BuildClaimsIdentity(jwt);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {    
                IsPersistent = true,        //Save cookie while Browser close
                ExpiresUtc = jwt.ValidTo,
                AllowRefresh = true,        //Allow refresh cookie
            };

            //  Sign in Cookie Authentication 
            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties
            );

            // Lưu JWT access token vào HttpOnly cookie để attach vào API calls
            SetSecureCookie(context, AccessTokenCookie, token.AccessToken,
                 jwt.ValidTo);

        
            SetSecureCookie(context, RefreshTokenCookie, token.RefreshToken,
                DateTime.UtcNow.AddDays(7));


            logger.LogInformation("User {UserId} signed in successfully", token.UserId);
        }


        public async Task SignOutAsync(HttpContext context)
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            DeleteCookie(context, AccessTokenCookie);
            DeleteCookie(context, RefreshTokenCookie);


            logger.LogInformation("User signed out");
        }



        // Read JWT access token from cookie when call API
        public string? GetAccessToken(HttpContext context)
            => context.Request.Cookies[AccessTokenCookie];

        public (string? UserId, string? RefreshToken) GetRefreshInfo(HttpContext context)
        {
            var accessToken = context.Request.Cookies[AccessTokenCookie];
            var refreshToken = context.Request.Cookies[RefreshTokenCookie];

            if (string.IsNullOrEmpty(accessToken))
                return (null, refreshToken);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(accessToken);
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            return (userId, refreshToken);
        }

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

        private static ClaimsIdentity BuildClaimsIdentity(JwtSecurityToken jwt)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? ""),
                new(ClaimTypes.Name,           jwt.Claims.FirstOrDefault(c => c.Type == "userName")?.Value ?? ""),
                new(ClaimTypes.Role,           jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? ""),
            };

            return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
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
                SameSite = SameSiteMode.Lax,    // Lax cho phép redirect từ external
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
