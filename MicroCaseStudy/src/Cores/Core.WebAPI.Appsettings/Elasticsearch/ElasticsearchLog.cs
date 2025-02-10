using System.Text.Json;
using Core.WebAPI.Appsettings.Wrappers;

namespace Core.WebAPI.Appsettings.Elasticsearch;

public class ElasticsearchLog
{
    public object Response { get; set; }
    public object Body { get; set; }
    public object HttpMethod { get; set; }
    public string Path { get; set; }
    public int StatusCode { get; set; }
    public object QueryParams { get; set; }
    public object UserId { get; set; }
    public object Email { get; set; }
    public object Username { get; set; }
    public object Name { get; set; }
    public object Surname { get; set; }
    public object Jti { get; set; }
    public DateTime Timestamp { get; set; }
    public object Url { get; set; }
    public object DataLog { get; set; }
    public object Schema { get; set; }

    private object _apiResultType = Wrappers.ApiResultType.Unspecified;

    public object ApiResultType
    {
        get { return (short)((ApiResultType)_apiResultType); }
        set
        {
            ApiResultType result;
            try
            {
                var data = JsonSerializer.Deserialize<Response>(value != null ? value.ToString() : string.Empty);
                result = data.apiResultType;
            }
            catch (Exception e)
            {
                result = Wrappers.ApiResultType.Unspecified;
            }

            _apiResultType = result;
        }
    }
}