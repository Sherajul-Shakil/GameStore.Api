using GameStore.Api.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GameStore.Api.Middleware
{
    public class UserValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public UserValidationMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            // 1. Check if the user is authenticated
            if (httpContext.User.Identity?.IsAuthenticated == true)
            {
                try
                {
                    // 2. Get the user ID from the token
                    var userIdString = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (int.TryParse(userIdString, out var userId))
                    {
                        // 3. Resolve GameStoreContext using IServiceProvider
                        using var scope = _serviceProvider.CreateScope();
                        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();

                        // 4. Check if the user exists in the database
                        var user = await dbContext.Users.FindAsync(userId);
                        if (user == null)
                        {
                            // 5. If the user doesn't exist, return unauthorized
                            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await httpContext.Response.WriteAsync("Unauthorized: User not found.");
                            return;
                        }
                    }
                    else
                    {
                        // 6. If the user ID is invalid, return unauthorized
                        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await httpContext.Response.WriteAsync("Unauthorized: Invalid user ID in token.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    // 7. Handle any errors during validation
                    Console.WriteLine($"Error validating user: {ex.Message}");
                    httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await httpContext.Response.WriteAsync("Internal server error during user validation.");
                    return;
                }
            }

            // 8. If user validation was successful, call the next middleware
            await _next(httpContext);
        }
    }
}