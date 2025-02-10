namespace Core.WebAPI.Appsettings;

public interface IUserSession<TUserId>
{
    public TUserId UserId { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Jti { get; set; }
    public string Body { get; set; }
    public string HttpMethod { get; set; }
    public string Path { get; set; }
    public string QueryParams { get; set; }
    public object DataLog { get; set; }
} 