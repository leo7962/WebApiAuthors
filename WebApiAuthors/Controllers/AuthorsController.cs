using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Context;
using WebApiAuthors.Dtos;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers;

[ApiController]
[Route("api/autores")]
public class AuthorsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public AuthorsController(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet] //api/autores
    public async Task<ActionResult<List<AuthorDto>>> Get()
    {
        var authors = await _context.Authors.ToListAsync();
        return Ok(_mapper.Map<List<AuthorDto>>(authors));
    }

    //[HttpGet("{id:int}/{param2?}")] se puede agregar varios parametros separados por /
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AuthorDtoWithBooks>> Get([FromRoute] int id)
    {
        var author = await _context.Authors.Include(y => y.BooksAuthors).ThenInclude(z => z.Book)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (author == null) return NotFound();

        return _mapper.Map<AuthorDtoWithBooks>(author);
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<List<AuthorDto>>> Get([FromRoute] string name)
    {
        var author = await _context.Authors.Where(x => x.Name.Contains(name)).ToListAsync();

        return Ok(_mapper.Map<List<AuthorDto>>(author));
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AuthorCreatedDto authorDto)
    {
        //Validaciones en el controlador
        var existsUser = await _context.Authors.AnyAsync(x => x.Name == authorDto.Name);

        if (existsUser) return BadRequest($"Ya existe un autor con el nombre {authorDto.Name}");

        var author = _mapper.Map<Author>(authorDto);

        _context.Add(author);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{id:int}")] //Api/autores/id = 1 o 2
    public async Task<IActionResult> Put(AuthorDto authorDto, int id)
    {
        if (authorDto.Id != id) return BadRequest("El id del autor no coincide con el id de la URL");

        var exists = await _context.Authors.AnyAsync(x => x.Id == id);
        if (!exists) return NotFound();

        _context.Update(authorDto);
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
