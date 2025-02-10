using System.Text.Json.Serialization;
using Core.Security.JWT.Dtos;

namespace IdentityService.Application.Features.Auths.Commands.RefreshToken;

public class RefreshedTokensDto
{
    public AccessToken AccessToken { get; set; }
    [JsonIgnore]
    public string RefreshToken { get; set; }

    public RefreshedTokensDto()
    {
        AccessToken = null!;
        RefreshToken = null!;
    }

    public RefreshedTokensDto(AccessToken accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}