namespace IdentityService.Application.Features.UserRoles.Queries.GetById;

public class GetByIdUserRoleDetailDto
{
    public int RoleId { get; set; }
    public string RoleValue { get; set; } = null!;
}