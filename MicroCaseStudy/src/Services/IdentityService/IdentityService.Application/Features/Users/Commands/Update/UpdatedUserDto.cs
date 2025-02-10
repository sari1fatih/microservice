namespace IdentityService.Application.Features.Users.Commands.Update;

public class UpdatedUserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    

    public UpdatedUserDto()
    {
        Name = string.Empty;
        Surname = string.Empty;
        Email = string.Empty;
    }

    public UpdatedUserDto(int id, string name, string surname, string email)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Email = email;
    }
}