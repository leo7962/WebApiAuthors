using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAuthors.Dtos;

namespace WebApiAuthors.Controllers.V1;

[ApiController]
[Route("api/v1")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class RoutesController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;

    public RoutesController(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    [HttpGet(Name = "ObtenerRoot")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DataHateoas>>> Get()
    {
        var datosHateoas = new List<DataHateoas>();

        var isAdmin = await _authorizationService.AuthorizeAsync(User, "isAdmin");

        datosHateoas.Add(new DataHateoas(Url.Link("ObtenerRoot", new { }), "self", "GET"));

        datosHateoas.Add(new DataHateoas(Url.Link("obtenerAutores", new { }), "autores",
            "GET"));
        if (!isAdmin.Succeeded) return datosHateoas;
        datosHateoas.Add(new DataHateoas(Url.Link("crearAutor", new { }), "autor-crear",
            "POST"));
        datosHateoas.Add(new DataHateoas(Url.Link("crearLibro", new { }), "libro-crear",
            "POST"));

        return datosHateoas;
    }
}
