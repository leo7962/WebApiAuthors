using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Context;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers;

[ApiController]
[Route("api/autores")]
public class AuthorsController : ControllerBase
{
    private readonly DataContext _context;

    public AuthorsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet] //api/autores
    [HttpGet("listado")] //api/autores/listado
    [HttpGet("/listado")] //listado
    public async Task<ActionResult<List<Author>>> Get()
    {
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
