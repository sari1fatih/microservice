using System.Diagnostics;
using System.Text.Json;
using Core.CrossCuttingConcerns.Exceptions.Handlers;
using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.CrossCuttingConcerns.Logging;
using Core.CrossCuttingConcerns.Serilog;
using Core.ElasticSearch;
using Core.ElasticSearch.Models;
using Core.Security.JWT;
using Core.WebAPI.Appsettings;
using Core.WebAPI.Appsettings.Elasticsearch;
using Core.WebAPI.Appsettings.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Context;

namespace Core.CrossCuttingConcerns.Exceptions;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HttpExceptionHandler _httpExceptionHandler;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly LoggerServiceBase _loggerServiceBase;
    private readonly IServiceProvider _serviceProvider;

    public ExceptionMiddleware(RequestDelegate next, IHttpContextAccessor httpContextAccessor,
        LoggerServiceBase loggerServiceBase, IServiceProvider serviceProvider)
    {
        _next = next;
        _httpExceptionHandler = new HttpExceptionHandler();
        _httpContextAccessor = httpContextAccessor;
        _loggerServiceBase = loggerServiceBase;
        _serviceProvider = serviceProvider;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await LogException(context, exception);
            await HandleElasticsearchRequestResponse(context, exception);
            await HandleExceptionAsync(context.Response, exception);
        }
    }

    private Task LogException(HttpContext context, Exception exception)
    {
        List<LogParameter> logParameters = new()
        {
            new LogParameter { Type = context.GetType().Name, Value = exception.ToString() }
        };


        LogDetailWithException logDetail = new()
        {
            ExceptionMessage = exception.Message,
            MethodName = _next.Method.Name,
            Parameters = logParameters,
            User = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "?"
        };

        string httpMethod = context.Request.Method;
        string path = context.Request.Path;
        string queryParams = context.Request.QueryString.HasValue
            ? context.Request.QueryString.Value
            : "No Query Parameters";

        string userId = string.Empty;

        var userIdCustom = context?.User?.Claims?.FirstOrDefault(x => x.Type == CustomClaimKeys.Id);

        if (userIdCustom != null)
        {
            userId = userIdCustom.Value;
        }

        using (var scope = _serviceProvider.CreateScope()) // Scope oluşturuluyor
        {
            var userSession =
                scope.ServiceProvider.GetRequiredService<IUserSession<int>>(); // Scoped servisi çözümlüyoruz

            using (LogContext.PushProperty("http_method", httpMethod))
            {
                LogContext.PushProperty("path", path);
                LogContext.PushProperty("query_params", queryParams);
                LogContext.PushProperty("body", userSession.Body); // scoped servisten alınan 'Body' bilgisi
                LogContext.PushProperty("user_id", userId);

                _loggerServiceBase.Error(JsonSerializer.Serialize(logDetail));
            }
        }

        return Task.CompletedTask;
    }

    private Task HandleExceptionAsync(HttpResponse response, Exception exception)
    {
        response.ContentType = "application/json";

        string message = string.Empty;

        switch (exception)
        {
            case BusinessException:
            case AuthorizationException:
            case NotFoundException:
            case ValidationException:

                message = exception.Message;
                break;
            default:
                message = "Please try again later";
                break;
        }

        Exception custom = new Exception(message);

        _httpExceptionHandler.Response = response;
        return _httpExceptionHandler.HandleExceptionAsync(custom);
    }

    private async Task HandleElasticsearchRequestResponse(HttpContext context, Exception exception)
    {
        var webApiConfiguration = context.RequestServices.GetService<WebApiConfiguration>();
        
        var userSession = context.RequestServices.GetService<IUserSession<int>>();

        if (webApiConfiguration != null && !webApiConfiguration.AllowedOrigins.Any(x=> userSession != null && x == userSession.Path))
        {
            var elasticSearch = context.RequestServices.GetService<IElasticSearch>(); 
            ElasticsearchLog elasticsearchLog = new ElasticsearchLog()
            {
                UserId = userSession!.UserId,
                Username = userSession.Username,
                Name = userSession.Name,
                Surname = userSession.Surname,
                Email = userSession.Email,
                Body = userSession.Body,
                StatusCode =_httpContextAccessor.HttpContext.Response.StatusCode,
                Response = exception.Message,
                Timestamp = DateTime.Now,
                Jti = userSession.Jti,
                QueryParams = userSession.QueryParams,
                Path = userSession.Path,
                HttpMethod = userSession.HttpMethod,
                ApiResultType = (short)ApiResultType.Unspecified,
                Schema = context.Request.Scheme,
                Url = context.Request.Host.Value
            };

            var result = await elasticSearch!.InsertAsync(new ElasticSearchInsertUpdateModel(
                Activity.Current?.Id,
                ElasticsearchIndex.IdentityServiceIndexName,
                elasticsearchLog
            ));
        }
    }
}