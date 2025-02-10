using FluentValidation;

namespace IdentityService.Application.Features.Roles.Commands.Create;

public class CreateRoleCommandValidator: AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(c => c.RoleValue).NotEmpty().MinimumLength(2);
    }
}
