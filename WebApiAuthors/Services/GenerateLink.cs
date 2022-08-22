using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using WebApiAuthors.Dtos;

namespace WebApiAuthors.Services;

public class GenerateLink
{
    private readonly IActionContextAccessor _actionContextAccessor;
    private readonly IAuthorizationService _authorizationService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GenerateLink(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor,
        IActionContextAccessor actionContextAccessor)
    {
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
        _actionContextAccessor = actionContextAccessor;
    }

    private IUrlHelper BuildUrlHelper()
    {
        var factory = _httpContextAccessor.HttpContext?.RequestServices.GetRequiredService<IUrlHelperFactory>();
        return factory?.GetUrlHelper(_actionContextAccessor.ActionContext);
    }

    private async Task<bool> IsAdmin()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var result = await _authorizationService.AuthorizeAsync(httpContext.User, "isAdmin");
        return result.Succeeded;
    }

    public async Task GenerateLinks(AuthorDto authorDto)
    {
        var isAdmin = await IsAdmin();
        var url = BuildUrlHelper();

        authorDto.Links.Add(new DataHateoas(url.Link("obtenerAutor", new { id = authorDto.Id }),
            "self",
            "GET"));
        if (isAdmin)
        {
            authorDto.Links.Add(new DataHateoas(url.Link("actualizarAutor", new { id = authorDto.Id }),
                "autor-Actualizar",
                "PUT"));
            authorDto.Links.Add(new DataHateoas(url.Link("BorrarAutor", new { id = authorDto.Id }),
                "self",
                "DELETE"));
        }
    }
}