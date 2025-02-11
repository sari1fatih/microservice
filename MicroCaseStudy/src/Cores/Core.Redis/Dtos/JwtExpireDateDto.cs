namespace Core.Redis.Dtos;

public class JwtExpireDateDto
{
    public string Jwt { get; set; }
    public DateTime ExpiresDate { get; set; }
}