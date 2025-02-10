using FluentValidation;

namespace IdentityService.Application.Features.UserRoles.Commands.Create;

public class CreateUserRoleValidator: AbstractValidator<CreateUserRoleCommand>
{
    public CreateUserRoleValidator()
    {
        RuleFor(c => c.UserId).NotNull();
        RuleFor(c => c.RoleId).GreaterThan(0);
    }
}
