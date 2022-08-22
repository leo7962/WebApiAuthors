using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAuthors.Helpers;

public class HateosFilterAttribute : ResultFilterAttribute
{
    protected static bool MayIncludeHateoas(ResultExecutingContext context)
    {
        var result = context.Result as ObjectResult;
        if (!SuccededResponse(result)) return false;

        var header = context.HttpContext.Request.Headers["includeHateoas"];
        if (header.Count == 0) return false;

        var value = header[0];
        return value.Equals("Y", StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool SuccededResponse(ObjectResult result)
    {
        if (result?.Value == null) return false;

        return !result.StatusCode.HasValue || result.StatusCode.Value.ToString().StartsWith("2");
    }
}