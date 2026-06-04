using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class EnforceMedicitasTokenAttribute : Attribute, IAsyncActionFilter
{
    private const string TokenEsperado = "Bearer medicitas2026";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var headerAuth))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Token no válido o no autorizado" });
            return;
        }

        var providedToken = headerAuth.ToString();
        if (string.IsNullOrWhiteSpace(providedToken) || providedToken != TokenEsperado)
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Token no válido o no autorizado" });
            return;
        }

        await next();
    }
}
