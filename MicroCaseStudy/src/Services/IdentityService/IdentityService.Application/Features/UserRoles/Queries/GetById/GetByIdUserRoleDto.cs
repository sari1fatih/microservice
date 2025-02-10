namespace IdentityService.Application.Features.UserRoles.Queries.GetById;

public class GetByIdUserRoleDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    public int RoleId { get; set; }
    public string RoleValue { get; set; } = null!;
}