namespace IdentityService.Application.Features.Auths.Commands.VerifyRegister;

public class VerifyForRegisterDto 
{
    public string ActivationCode { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Username { get; set; }
    
    public string Password { get; set; } 

    public VerifyForRegisterDto()
    {
        this.ActivationCode = string.Empty;
        this.Email = string.Empty;
        this.Password = string.Empty;
        this.Name = string.Empty;
        this.Surname = string.Empty;
        this.Username = string.Empty; 
    }

    public VerifyForRegisterDto(string email, string name, string surname, string username, string password)
    {
        Email = email;
        Name = name;
        Surname = surname;
        Username = username;
        Password = password; 
    }
}