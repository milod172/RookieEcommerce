using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using NovaFashion.API.Entities;
using NovaFashion.API.Entities.Enum;
using NovaFashion.SharedViewModels.AuthenticationDtos;

namespace NovaFashion.API.Features.Authentications
{
    public class RegisterRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class RegisterValidator : Validator<RegisterRequest>
    {
        public const string FirstNameRequired = "Họ không được để trống";
        public const string LastNameRequired = "Tên không được để trống";
        public const string EmailRequired = "Email không được để trống";
        public const string EmailInvalid = "Email không hợp lệ";
        public const string PasswordRequired = "Mật khẩu không được để trống";
        public const string ConfirmPasswordMismatch = "Mật khẩu xác nhận không khớp";

        public RegisterValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage(FirstNameRequired)
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage(LastNameRequired)
                .MaximumLength(50);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(EmailRequired)
                .EmailAddress().WithMessage(EmailInvalid);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(PasswordRequired);

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password).WithMessage(ConfirmPasswordMismatch);
        }
    }

    public class RegisterEndpoint(UserManager<ApplicationUser> userManager) : Endpoint<RegisterRequest, RegisterDto>
    {
        
        public override void Configure()
        {
            Post("/register");
            Group<AuthGroup>();
            AllowAnonymous();
        }

        public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
        {
            var user = new ApplicationUser
            {
                FirstName = req.FirstName,
                LastName = req.LastName,
                Email = req.Email,
                UserName = req.Email, 
                CreatedDate = DateTime.UtcNow,
            };

            var result = await userManager.CreateAsync(user, req.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    AddError(error.Description);
            }

            ThrowIfAnyErrors();

      
            await userManager.AddToRoleAsync(user, Role.Customer.ToString());

            await Send.OkAsync(new RegisterDto
            {
                Message = "Đăng ký thành công"
            }, cancellation: ct);
        }
    }
}
