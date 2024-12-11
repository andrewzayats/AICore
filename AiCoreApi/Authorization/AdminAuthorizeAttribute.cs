using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AiCoreApi.Authorization;

public class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        var role = user.FindFirstValue(ClaimTypes.Role);
        if (role == "Admin")
            return;
        context.Result = new UnauthorizedResult();
    }
}
