namespace Core.WebAPI.Appsettings.Wrappers;

public class BaseService : IBaseService
{
    public Response<T> CreateSuccessResult<T>(T result, string message)
    {
        return new Response<T>()
        {
            ApiResultType = ApiResultType.Success,
            Data = result,
            Message = new List<string>() { message },
        };
    }

    public Response<T> CreateSuccessResult<T>(T result, IEnumerable<string> messages)
    {
        return new Response<T>()
        {
            ApiResultType = ApiResultType.Success,
            Data = result,
            Message = messages
        };
    }
}