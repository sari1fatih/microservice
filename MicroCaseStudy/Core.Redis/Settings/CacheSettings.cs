namespace Core.Redis.Settings;

public class CacheSettings
{
    public int SlidingExpiration { get; set; }
    public int AbsoluteExpiration { get; set; }
}