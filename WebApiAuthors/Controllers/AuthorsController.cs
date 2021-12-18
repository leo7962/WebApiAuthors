using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Context;
using WebApiAuthors.Entities;
using WebApiAuthors.Services;

namespace WebApiAuthors.Controllers;

[ApiController]
[Route("api/autores")]
public class AuthorsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly ILogger<AuthorsController> _logger;
    private readonly IService _service;
    private readonly ServiceScoped _serviceScoped;
    private readonly ServiceSingleton _serviceSingleton;
    private readonly ServiceTransient _serviceTransient;

    public AuthorsController(DataContext context, IService service, ServiceTransient serviceTransient,
        ServiceScoped serviceScoped, ServiceSingleton serviceSingleton, ILogger<AuthorsController> logger)
    {
        _context = context;
        _service = service;
        _serviceTransient = serviceTransient;
        _serviceScoped = serviceScoped;
        _serviceSingleton = serviceSingleton;
        _logger = logger;
    }

    [HttpGet("GUID")]
    public ActionResult ObtenerGuids()
    {
        return Ok(
            new
            {
                AuthorsControllerTransient = _serviceTransient.Guid,
                ServiceA_Transient = _service.GetTransient(),
                AuthorsControllerScoped = _serviceScoped.Guid,
                ServiceA_Scoped = _service.GetScoped(),
                AuthorsControllerSingleton = _serviceSingleton.Guid,
                ServiceA_Singleton = _service.GetSingleton()
            }
        );
    }

    [HttpGet] //api/autores
    [HttpGet("listado")] //api/autores/listado
    [HttpGet("/listado")] //listado
    public async Task<ActionResult<List<Author>>> Get()
    {
        _logger.LogInformation("Estamos obteniendo una lista de autores");
        _logger.LogWarning("Este es un mensaje de prueba");
        return await _context.Authors.Include(x => x.Books).ToListAsync();
    }

    [HttpGet("primero")] //api/autores/primero?nombre=leonardo&apellido=hernandez
    public async Task<ActionResult<Author>> FirstAuthor([FromHeader] int value, [FromQuery] string name)
    {
        return await _context.Authors.Include(x => x.Books).FirstOrDefaultAsync();
    }

    //[HttpGet("{id:int}/{param2?}")] se puede agregar varios parametros separados por /
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Author>> Get([FromRoute] int id)
    {
        var author = await _context.Authors.Include(x => x.Books).FirstOrDefaultAsync(x => x.Id == id);
        if (author == null) return NotFound();

        return author;
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<Author>> Get([FromRoute] string name)
    {
        var author = await _context.Authors.Include(x => x.Books).FirstOrDefaultAsync(x => x.Name.Contains(name));
        if (author == null) return NotFound();

        return Ok(author);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Author author)
    {
        //Validaciones en el controlador
        var existsUser = await _context.Authors.AnyAsync(x => x.Name == author.Name);
        if (existsUser) return BadRequest($"Ya existe un autor con el nombre {author.Name}");

        _context.Add(author);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{id:int}")] //Api/autores/id = 1 o 2
    public async Task<IActionResult> Put(Author author, int id)
    {
        if (author.Id != id) return BadRequest("El id del autor no coincide con el id de la URL");

        var exists = await _context.Authors.AnyAsync(x => x.Id == id);
        if (!exists) return NotFound();

        _context.Update(author);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:int}")] //Api/autores/2
    public async Task<IActionResult> Delete(int id)
    {
        var exists = await _context.Authors.AnyAsync(x => x.Id == id);
        if (!exists) return NotFound();

        _context.Remove(new Author {Id = id});
        await _context.SaveChangesAsync();
        return Ok();
    }
}
