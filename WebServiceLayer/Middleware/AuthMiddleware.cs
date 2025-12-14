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
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();

        if (!string.IsNullOrEmpty(authHeader))
        {
            // Try to parse as user ID first (number), then as username
            if (int.TryParse(authHeader, out int userId))
            {
                // Get user by ID
                var user = _dataService.GetUser(userId);
                if (user != null)
                {
                    context.Items["User"] = user;
                }
            }
            else
            {
                // Get user by username
                var user = _dataService.GetUser(authHeader);
                if (user != null)
                {
                    context.Items["User"] = user;
                }
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