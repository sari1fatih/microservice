namespace IdentityService.Application.Features.Auths.Commands.Redis;

public class RegisterRedisDto
{
    public string Mail { get; set; }
    public string ActivationCode { get; set; }
}