using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using WebProject.ExternalServices.Interfaces;

namespace WebProject.Middlewares;

public class SessionValidationMiddleware(RequestDelegate _next)
{
    public async Task InvokeAsync(HttpContext context, ISessionService sessionService)
    {
        if(context.User.Identity?.IsAuthenticated == true)
        {
            string userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("ClaimTypes NameIdentifier not found");
            
            string sessionToken = context.User.FindFirstValue("SessionToken") ?? throw new Exception($"SessionToken not found for user with id: {userId}");

            if(!await sessionService.IsValidAsync(userId, sessionToken))
            {
                await context.SignOutAsync();
                context.Response.Redirect("/Auth/Login");
                return;
            }
        }

        await _next(context);
    }

}
