using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.CustomerSite.Services.Auth;

namespace NovaFashion.CustomerSite.Pages.Auths
{
    public class LogoutModel(JwtCookieService jwtCookieService, AuthApiClient authApi) : PageModel
    {
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // revoke refresh token
            var accessToken = jwtCookieService.GetAccessToken(HttpContext);
            if (!string.IsNullOrEmpty(accessToken))
            {
                await authApi.RevokeTokenAsync(accessToken);
            }

            // delete cookies
            await jwtCookieService.SignOutAsync(HttpContext);

            return RedirectToPage("/Index");
        }
    }
}
