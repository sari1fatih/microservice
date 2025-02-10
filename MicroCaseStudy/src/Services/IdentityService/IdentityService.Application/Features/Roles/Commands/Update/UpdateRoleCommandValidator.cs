using FluentValidation;

namespace IdentityService.Application.Features.Roles.Commands.Update;

public class UpdateRoleCommandValidator: AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(c => c.RoleValue).NotEmpty().MinimumLength(2);
    }
}
