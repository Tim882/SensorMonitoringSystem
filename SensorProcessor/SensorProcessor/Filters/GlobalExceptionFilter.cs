using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SensorProcessor.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Unhandled exception occurred");

        context.Result = new ObjectResult(new
        {
            error = "An unexpected error occurred",
            requestId = context.HttpContext.TraceIdentifier
        })
        {
            StatusCode = 500
        };

        context.ExceptionHandled = true;
    }
}