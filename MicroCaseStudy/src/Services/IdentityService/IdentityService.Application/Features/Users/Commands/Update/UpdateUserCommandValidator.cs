using FluentValidation;

namespace IdentityService.Application.Features.Users.Commands.Update;

public class UpdateUserCommandValidator: AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MinimumLength(2);
        RuleFor(c => c.Surname).NotEmpty().MinimumLength(2); 
    }
}
