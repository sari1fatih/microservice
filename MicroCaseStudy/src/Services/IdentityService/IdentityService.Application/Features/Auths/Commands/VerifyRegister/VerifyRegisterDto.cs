using System.Text.Json.Serialization;
using Core.Security.JWT.Dtos;

namespace IdentityService.Application.Features.Auths.Commands.VerifyRegister;

public class VerifyRegisterDto
{
    public AccessToken AccessToken { get; set; }
    [JsonIgnore] 
    public string RefreshToken { get; set; } 


    public LoggedHttpResponse ToHttpResponse()
    {
        return new() { AccessToken = AccessToken};
    }

    public class LoggedHttpResponse
    {
        public AccessToken? AccessToken { get; set; } 
    }
}