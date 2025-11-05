using DataServiceLayer;
using Microsoft.AspNetCore.Builder;

namespace WebServiceLayer.CustomMiddleware;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDataService _dataService;

    public AuthMiddleware(RequestDelegate next, IDataService dataService)
    {
        _next = next;
        _dataService = dataService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userName = context.Request.Headers.Authorization.FirstOrDefault();

        if (!string.IsNullOrEmpty(userName))
        {
            var user = _dataService.GetUser(userName);
            if (user != null)
            {
                context.Items["User"] = user;
            }
        }

        await _next(context);
    }
}

public static class TeamAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseAuth(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthMiddleware>();
    }
}