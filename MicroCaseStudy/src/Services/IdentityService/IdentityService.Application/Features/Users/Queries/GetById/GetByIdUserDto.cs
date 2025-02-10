namespace IdentityService.Application.Features.Users.Queries.GetById;

public class GetByIdUserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }

    public GetByIdUserDto()
    {
        Username = string.Empty;
        Name = string.Empty;
        Surname = string.Empty;
        Email = string.Empty;
    }

    public GetByIdUserDto(int id, string username, string name, string surname, string email)
    {
        Id = id;
        Username = username;
        Name = name;
        Surname = surname;
        Email = email;
    }
}