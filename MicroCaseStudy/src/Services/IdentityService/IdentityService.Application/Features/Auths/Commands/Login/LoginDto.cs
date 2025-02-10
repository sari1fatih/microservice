using System.Text.Json.Serialization;
using Core.Security.JWT.Dtos;
using IdentityService.Domain.Entities;

namespace IdentityService.Application.Features.Auths.Commands.Login;

public class LoginDto
{
    public AccessToken? AccessToken { get; set; }
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