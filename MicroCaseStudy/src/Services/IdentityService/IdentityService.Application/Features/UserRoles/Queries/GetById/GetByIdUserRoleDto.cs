namespace IdentityService.Application.Features.UserRoles.Queries.GetById;

public class GetByIdUserRoleDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
}