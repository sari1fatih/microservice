  namespace Core.WebAPI.Appsettings.Wrappers;

public interface IBaseService
{
    public Response<T> CreateSuccessResult<T>(T result, string message);

    public Response<T> CreateSuccessResult<T>(T result, IEnumerable<string> messages);
}