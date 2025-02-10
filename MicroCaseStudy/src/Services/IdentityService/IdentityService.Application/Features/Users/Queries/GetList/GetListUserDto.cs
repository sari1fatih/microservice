namespace IdentityService.Application.Features.Users.Queries.GetList;

public class GetListUserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }

    public GetListUserDto()
    {
        Username = string.Empty;
        Name = string.Empty;
        Surname = string.Empty;
        Email = string.Empty;
    }

    public GetListUserDto(int id, string username, string name, string surname, string email)
    {
        Id = id;
        Username = username;
        Name = name;
        Surname = surname;
        Email = email;
    }
}