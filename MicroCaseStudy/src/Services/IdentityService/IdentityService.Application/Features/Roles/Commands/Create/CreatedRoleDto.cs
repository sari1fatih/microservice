namespace IdentityService.Application.Features.Roles.Commands.Create;

public class CreatedRoleDto
{
    public int Id { get; set; }
    public string RoleValue { get; set; }

    public CreatedRoleDto()
    {
        RoleValue = string.Empty;
    }

    public CreatedRoleDto(int id, string roleValue)
    {
        Id = id;
        RoleValue = roleValue;
    }
}