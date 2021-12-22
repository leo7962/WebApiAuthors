using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAuthors.Filters;

public class MyFilterAction : IActionFilter
{
    private readonly ILogger<MyFilterAction> _logger;

    public MyFilterAction(ILogger<MyFilterAction> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        _logger.LogInformation("Antes de ejecutar la acción");
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        _logger.LogInformation("Después de ejecutar la acción");
    }
}