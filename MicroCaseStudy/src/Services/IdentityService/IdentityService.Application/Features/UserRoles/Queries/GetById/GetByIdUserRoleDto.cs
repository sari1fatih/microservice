namespace IdentityService.Application.Features.UserRoles.Queries.GetById;

public class GetByIdUserRoleDto
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public List<GetByIdUserRoleDetailDto> Roles { get; set; } = new List<GetByIdUserRoleDetailDto>();
}