using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebProject.Models;

namespace WebProject.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AuthorizePermissionAttribute(int page, int access = (int)PageAccess.Read) : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var permissions = user.FindAll("Permission").Select(perm => int.Parse(perm.Value));

        bool isAllowed = permissions.Any(perm => (perm & page) == page && (perm & access) == access);

        if (!isAllowed)
            context.Result = new ForbidResult();
    }
}
