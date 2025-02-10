namespace IdentityService.Application.Features.Roles.Queries.GetList;

public class GetListRoleDto
{
    public int Id { get; set; }
    public string RoleValue { get; set; }

    public GetListRoleDto()
    {
        RoleValue = string.Empty;
    }

    public GetListRoleDto(int id, string roleValue)
    {
        Id = id;
        RoleValue = roleValue;
    }
}