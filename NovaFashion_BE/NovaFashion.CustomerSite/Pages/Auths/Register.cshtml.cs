using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NovaFashion.CustomerSite.Services.Auth;
using NovaFashion.SharedViewModels.AuthenticationDtos;

namespace NovaFashion.CustomerSite.Pages.Auths
{
    public class RegisterModel(AuthApiClient authApi) : PageModel
    {
        [BindProperty]
        public RegisterRequest Input { get; set; }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
                return Page();

            var (data, errors) = await authApi.RegisterAsync(Input);

            if (errors is not null)
            {
                foreach (var (field, messages) in errors)
                {
                    // "confirm_password" → "Input.ConfirmPassword"
                    // ""                 → ""  (lỗi chung, hiện qua validation-summary)
                    var modelKey = string.IsNullOrEmpty(field)
                        ? string.Empty
                        : $"Input.{SnakeToPascal(field)}";

                    foreach (var msg in messages)
                        ModelState.AddModelError(modelKey, msg);
                }

                return Page();
            }

            if (data is not null)
                return RedirectToPage("/Auths/Login");

            ModelState.AddModelError("", "Đăng ký thất bại");
            return Page();

        }

        private static string SnakeToPascal(string snake) =>
        string.Concat(
            snake.Split('_')
                 .Select(w => w.Length > 0 ? char.ToUpper(w[0]) + w[1..] : w)
        );
    }
}
