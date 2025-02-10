namespace Core.WebAPI.Appsettings.Wrappers;

public class Response
{
    public ApiResultType apiResultType { get; set; } = ApiResultType.Unspecified;
}

public class Response<T>
{
    public ApiResultType ApiResultType { get; set; } = ApiResultType.Unspecified;

    public T Data { get; set; }
    public IEnumerable<string> Message { get; set; }

    public Response()
    {
    }
 
    public Response(T data, IEnumerable<string> message)
    {
        ApiResultType = ApiResultType.Success;
        Data = data;
        Message = message;
    }

    public Response(T data, string message)
    {
        ApiResultType = ApiResultType.Success;
        Data = data;
        Message = new[] { message };
    }

    public Response(ApiResultType apiResultType, IEnumerable<string> message)
    {
        ApiResultType = apiResultType;
        Message = message;
    }

    public Response(ApiResultType apiResultType, string message)
    {
        ApiResultType = apiResultType;
        Message = new[] { message };
    }
}