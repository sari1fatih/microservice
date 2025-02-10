namespace IdentityService.Application.Features.UserRoles.Commands.Create;

public class CreatedUserRoleDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
}