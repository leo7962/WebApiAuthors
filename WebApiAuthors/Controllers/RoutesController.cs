using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApiAuthors.Dtos;

namespace WebApiAuthors.Controllers;

[ApiController]
[Route("api")]
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

        datosHateoas.Add(new DataHateoas(link: Url.Link("ObtenerRoot", new { }), description: "self", method: "GET"));

        datosHateoas.Add(new DataHateoas(link: Url.Link("obtenerAutores", new { }), description: "autores",
            method: "GET"));
        if (isAdmin.Succeeded)
        {
            datosHateoas.Add(new DataHateoas(link: Url.Link("crearAutor", new { }), description: "autor-crear",
                method: "POST"));
            datosHateoas.Add(new DataHateoas(link: Url.Link("crearLibro", new { }), description: "libro-crear",
                method: "POST"));
        }

        return datosHateoas;
    }
}
