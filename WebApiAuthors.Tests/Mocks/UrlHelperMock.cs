using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace WebApiAuthors.Tests.Mocks;

public class UrlHelperMock : IUrlHelper
{
    public string? Action(UrlActionContext actionContext)
    {
        return "";
    }

    public string? Content(string? contentPath)
    {
        return "";
    }

    public bool IsLocalUrl(string? url)
    {
        return true;
    }

    public string? RouteUrl(UrlRouteContext routeContext)
    {
        return "";
    }

    public string? Link(string? routeName, object? values)
    {
        return "";
    }

    public ActionContext ActionContext { get; }
}
