namespace IdentityService.Application.Features.UserRoles.Queries.GetList;

public class GetListUserRoleDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public List<GetListUserRoleDetailDto> Roles { get; set; } = new List<GetListUserRoleDetailDto>();
}