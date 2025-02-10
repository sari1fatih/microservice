using System.Text.RegularExpressions;
using FluentValidation;

namespace IdentityService.Application.Features.Auths.VerifyResetPassword;

public class VerifyResetPasswordValidator : AbstractValidator<VerifyResetPasswordCommand>
{
    public VerifyResetPasswordValidator()
    {
        RuleFor(c => c.ActivationCode).NotEmpty();
        RuleFor(c => c.Password)
            .NotEmpty()
            .MinimumLength(6)
            .Must(StrongPassword)
            .WithMessage(
                "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character."
            );
    }

    private bool StrongPassword(string value)
    {
        Regex strongPasswordRegex = new("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",
            RegexOptions.Compiled);

        return strongPasswordRegex.IsMatch(value);
    }
}