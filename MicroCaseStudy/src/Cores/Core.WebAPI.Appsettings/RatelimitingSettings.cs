namespace Core.WebAPI.Appsettings;

public class RatelimitingSettings
{
    public int PermitLimit { get; set; }
    public int WindowSeconds { get; set; }
}