using System.Text.RegularExpressions;
using FluentValidation;

namespace IdentityService.Application.Features.Auths.Commands.VerifyRegister;

public class VerifyRegisterCommandValidator : AbstractValidator<VerifyRegisterCommand>
{
    public VerifyRegisterCommandValidator()
    {
        RuleFor(c => c.VerifyForRegisterDto.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.VerifyForRegisterDto.ActivationCode).NotEmpty();
        RuleFor(c => c.VerifyForRegisterDto.Name).NotEmpty();
        RuleFor(c => c.VerifyForRegisterDto.Username).NotEmpty();
        RuleFor(c => c.VerifyForRegisterDto.Surname).NotEmpty();
           
        RuleFor(c => c.VerifyForRegisterDto.Password)
            .NotEmpty()
            .MinimumLength(6)
            .Must(StrongPassword)
            .WithMessage(
                "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character."
            );
    }
    
    private bool StrongPassword(string value)
    {
        Regex strongPasswordRegex = new("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", RegexOptions.Compiled);
   
        return strongPasswordRegex.IsMatch(value);
    }
} 