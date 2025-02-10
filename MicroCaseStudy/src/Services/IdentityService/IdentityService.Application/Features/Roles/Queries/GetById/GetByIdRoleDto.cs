namespace IdentityService.Application.Features.Roles.Queries.GetById;

public class GetByIdRoleDto
{
    public int Id { get; set; }
    public string RoleValue { get; set; }

    public GetByIdRoleDto()
    {
        RoleValue = string.Empty;
    }

    public GetByIdRoleDto(int id, string roleValue)
    {
        Id = id;
        RoleValue = roleValue;
    }
}