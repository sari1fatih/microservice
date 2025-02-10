namespace Core.WebAPI.Appsettings;

public class WebApiConfiguration
{
    public string ApiDomain { get; set; }
    public string[] AllowedOrigins { get; set; }
    public string[] ExcludedPaths { get; set; }

    public WebApiConfiguration()
    {
        ApiDomain = string.Empty;
        AllowedOrigins = Array.Empty<string>();
        ExcludedPaths = Array.Empty<string>();
    }

    public WebApiConfiguration(string apiDomain, string[] allowedOrigins, string[] excludedPaths)
    {
        ApiDomain = apiDomain;
        AllowedOrigins = allowedOrigins;
        ExcludedPaths = excludedPaths;
    }
}