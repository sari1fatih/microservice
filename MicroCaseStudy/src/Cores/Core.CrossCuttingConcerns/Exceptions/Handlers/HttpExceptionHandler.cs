using System.Text.Json;
using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.WebAPI.Appsettings.Wrappers;
using Microsoft.AspNetCore.Http;

namespace Core.CrossCuttingConcerns.Exceptions.Handlers;

public class HttpExceptionHandler : ExceptionHandler
{
    private HttpResponse? _response;

     public HttpResponse Response
    {
        get => _response ?? throw new ArgumentNullException(nameof(_response));
        set => _response = value;
    }

    protected override Task HandleException(BusinessException businessException)
    {
        _response.StatusCode = StatusCodes.Status400BadRequest;
        string details = JsonSerializer.Serialize(new Response<BusinessException>(ApiResultType.Warning,businessException.Message));
        return _response.WriteAsync(details);
    }

    protected override Task HandleException(ValidationException validationException)
    {
        _response.StatusCode = StatusCodes.Status400BadRequest;
        string details = JsonSerializer.Serialize(new Response<ValidationException>(ApiResultType.Error, validationException.Errors.SelectMany(x => x.Errors)));
        return _response.WriteAsync(details);
    }

    protected override Task HandleException(AuthorizationException authorizationException)
    {
        _response.StatusCode = StatusCodes.Status400BadRequest;
        string details = JsonSerializer.Serialize(new Response<ValidationException>(ApiResultType.Error, authorizationException.Message));
        return _response.WriteAsync(details);
    }

    protected override Task HandleException(NotFoundException notFoundException)
    {
        _response.StatusCode = StatusCodes.Status400BadRequest;
        string details = JsonSerializer.Serialize(new Response<ValidationException>(ApiResultType.Error, notFoundException.Message));
        return _response.WriteAsync(details);
    }


    protected override Task HandleException(Exception exception)
    {
        _response.StatusCode = StatusCodes.Status500InternalServerError;
        string details = JsonSerializer.Serialize(new Response<Exception>(ApiResultType.Error,exception.Message));
        return _response.WriteAsync(details);
    }
    

}