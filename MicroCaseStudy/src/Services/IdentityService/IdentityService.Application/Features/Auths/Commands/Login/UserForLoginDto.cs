namespace IdentityService.Application.Features.Auths.Commands.Login;

public class UserForLoginDto
{
    public string Email { get; set; }

    public string Password { get; set; }

    
    public UserForLoginDto()
    {
        this.Email = string.Empty;
        this.Password = string.Empty;
    }

    public UserForLoginDto(string email, string password)
    {
        this.Email = email;
        this.Password = password;
    }
}