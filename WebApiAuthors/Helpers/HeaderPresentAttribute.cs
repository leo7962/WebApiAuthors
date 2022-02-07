using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace WebApiAuthors.Helpers;

public class HeaderPresentAttribute : Attribute, IActionConstraint
{
    private readonly string _header;
    private readonly string _value;

    public HeaderPresentAttribute(string header, string value)
    {
        _header = header;
        _value = value;
    }

    public bool Accept(ActionConstraintContext context)
    {
        var headers = context.RouteContext.HttpContext.Request.Headers;
        if (!headers.ContainsKey(_header)) return false;

        return string.Equals(headers[_header], _value, StringComparison.OrdinalIgnoreCase);
    }

    public int Order => 0;
}
