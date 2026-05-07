using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.CustomerSite.Services.Auth;

namespace NovaFashion.CustomerSite.Pages.Auths
{
    public class LoginModel(
        AuthApiClient authApiClient,
        JwtCookieService jwtCookieService,
        ILogger<LoginModel> logger) : PageModel
    {

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public string? ReturnUrl { get; set; }

        public void OnGet(string? returnUrl = null, bool expired = false)
        {
            ReturnUrl = returnUrl;
            if (expired)
                ErrorMessage = "Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại.";
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return Page();

            var token = await authApiClient.LoginAsync(Email, Password);

            if (token is null)
            {
                ErrorMessage = "Email hoặc mật khẩu không đúng.";
                return Page();
            }

            await jwtCookieService.SignInAsync(HttpContext, token);

            logger.LogInformation("User {Email} logged in", Email);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToPage("/Index");
        }
    }
}
