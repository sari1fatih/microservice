using System.Diagnostics;
using System.Text;
using Core.ElasticSearch;
using Core.ElasticSearch.Models;
using Core.WebAPI.Appsettings;
using Core.WebAPI.Appsettings.Elasticsearch;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace IdentityService.Api.Attributes;

public class ElasticsearchRequestResponseAttribute : ActionFilterAttribute
{
    public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var userSession = context.HttpContext.RequestServices.GetService<IUserSession<int>>();
        
        var webApiConfiguration = context.HttpContext.RequestServices.GetService<IOptions<WebApiConfiguration>>().Value;

        if (webApiConfiguration != null && !webApiConfiguration.ExcludedPaths.Any(x => x == userSession.Path))
        {
            var elasticSearch = context.HttpContext.RequestServices.GetService<IElasticSearch>();

            // 2. Response Body'sini Oku (Action sonuçlandıktan sonra)
            var response = context.HttpContext.Response;

            // Response Body'yi MemoryStream ile sarmalayalım
            var originalBodyStream = response.Body;
            using (var memoryStream = new MemoryStream())
            {
                // Response Body'yi MemoryStream'e yönlendiriyoruz
                response.Body = memoryStream;

                // Action sonrası response yazılacak ve MemoryStream'e aktarılacak
                await next(); // Action sonrası response yazılacak

                // Response'yi okuma
                memoryStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await ReadStreamAsync(memoryStream);

                // Response'yi orijinal stream'e tekrar yazıyoruz
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalBodyStream);
                ElasticsearchLog elasticsearchLog = new ElasticsearchLog()
                {
                    UserId = userSession.UserId,
                    Username = userSession.Username,
                    Name = userSession.Name,
                    Surname = userSession.Surname,
                    Email = userSession.Email,
                    Body = userSession.Body,
                    StatusCode = context.HttpContext.Response.StatusCode,
                    Response = responseBody,
                    Timestamp = DateTime.Now,
                    Jti = userSession.Jti,
                    QueryParams = userSession.QueryParams,
                    Path = userSession.Path,
                    HttpMethod = userSession.HttpMethod,
                    ApiResultType = responseBody,
                    Schema = context.HttpContext.Request.Scheme,
                    Url = context.HttpContext.Request.Host.Value,
                    DataLog = userSession.DataLog,
                };

                var a =await elasticSearch.InsertAsync(new ElasticSearchInsertUpdateModel(
                    Activity.Current?.Id,
                    ElasticsearchIndex.IdentityServiceIndexName,
                    elasticsearchLog
                ));
            }
        }
        else
        {
            await next();
        }
    }

    private async Task<string> ReadStreamAsync(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }
}