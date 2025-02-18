namespace IdentityService.Application.Features.UserRoles.Queries.GetList;

public class GetListUserRoleDetailDto
{
    public int RoleId { get; set; }
    public string RoleValue { get; set; } = null!;
}