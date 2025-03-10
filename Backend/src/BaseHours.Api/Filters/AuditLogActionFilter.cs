using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace BaseHours.Infrastructure.Filters;

public class AuditLogActionFilter : IActionFilter
{
    private readonly ILogger<AuditLogActionFilter> _logger;

    public AuditLogActionFilter(ILogger<AuditLogActionFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var controller = context.Controller.GetType().Name;
        var action = context.ActionDescriptor.DisplayName;
        var method = context.HttpContext.Request.Method;
        var path = context.HttpContext.Request.Path;
        var queryParams = context.HttpContext.Request.QueryString;

        _logger.LogInformation("Request: {Method} {Path}{QueryParams} in {Controller} - {Action}",
            method, path, queryParams, controller, action);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        var statusCode = context.HttpContext.Response.StatusCode;
        _logger.LogInformation("Response Status Code: {StatusCode}", statusCode);
    }
}
