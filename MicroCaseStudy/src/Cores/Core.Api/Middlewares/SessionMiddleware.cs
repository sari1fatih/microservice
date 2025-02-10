using Core.Security.JWT;
using Core.WebAPI.Appsettings;
using Microsoft.AspNetCore.Http;

namespace Core.Api.Middlewares;

public class SessionMiddleware
{
    private readonly RequestDelegate _next;

    public SessionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserSession<int> userSession)
    {
        userSession.HttpMethod = context?.Request?.Method ?? string.Empty;
        userSession.Path = context?.Request?.Path ?? string.Empty;
        userSession.QueryParams= context?.Request != null && context.Request.QueryString.HasValue
            ? context.Request.QueryString.Value
            : "No Query Parameters";
        userSession.Body = await GetRequestBody(context);
        if (context.User.Identities.Any(id => id.IsAuthenticated))
        {
            FillWithUserSession(context, userSession);
            await _next.Invoke(context);
        }
        else
        {
            await _next.Invoke(context);
        }
    }

    private void FillWithUserSession(HttpContext context, IUserSession<int> userSession)
    {
        var nameIdentifierClaim = context.User.Claims.FirstOrDefault(x => x.Type == CustomClaimKeys.Id);

        if (nameIdentifierClaim != null)
        {
            if (int.TryParse(nameIdentifierClaim.Value, out int userId))
            {
                userSession.UserId = userId;
            } 
        }
        var emailClaim = context.User.Claims.FirstOrDefault(x => x.Type == CustomClaimKeys.Mail);

        if (emailClaim != null)
        {
            userSession.Email = emailClaim.Value;
        }

        var nameIdClaim = context.User.Claims.FirstOrDefault(x => x.Type == CustomClaimKeys.Username);

        if (nameIdClaim != null)
        {
            userSession.Username = nameIdClaim.Value;
        }
 
        var nameClaim = context.User.Claims.FirstOrDefault(x => x.Type == CustomClaimKeys.Name);

        if (nameClaim != null)
        {
            userSession.Name = nameClaim.Value;
        }

        var familyNameClaim = context.User.Claims.FirstOrDefault(x => x.Type == CustomClaimKeys.Surname);

        if (familyNameClaim != null)
        {
            userSession.Surname = familyNameClaim.Value;
        }

        var jtiClaim = context.User.Claims.FirstOrDefault(x => x.Type == CustomClaimKeys.Jti);

        if (jtiClaim != null)
        {
            userSession.Jti = jtiClaim.Value;
        }
 
    }
    private async Task<string> GetRequestBody(HttpContext context)
    {
        context.Request.EnableBuffering(); // Body'nin yeniden okunmasını sağlar
        using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
        {
            string body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0; // Body pozisyonunu başa döndür
            return body;
        }
    }
}