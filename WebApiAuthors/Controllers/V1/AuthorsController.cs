﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Context;
using WebApiAuthors.Dtos;
using WebApiAuthors.Entities;
using WebApiAuthors.Helpers;

namespace WebApiAuthors.Controllers.V1;

[ApiController]
[Route("api/autores")]
//[Route("api/v1/autores")]
[HeaderPresent("x-version", "1")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "IsAdmin")]
// [ApiConventionType(typeof(DefaultApiConventions))]
public class AuthorsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public AuthorsController(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet(Name = "obtenerAutoresv1")] //api/autores
    [AllowAnonymous]
    [ServiceFilter(typeof(HateoasAuthorFilterAttribute))]
    public async Task<ActionResult<List<AuthorDto>>> Get([FromQuery] PaginationDto paginationDto)
    {
        var queryable = _context.Authors.AsQueryable();
        await HttpContext.InsertParametersPaginationHeader(queryable);
        var authors = await queryable.OrderBy(author => author.Name).Page(paginationDto).ToListAsync();
        return Ok(_mapper.Map<List<AuthorDto>>(authors));
    }

    //[HttpGet("{id:int}/{param2?}")] se puede agregar varios parametros separados por /
    [HttpGet("{id:int}", Name = "obtenerAutorv1")]
    [AllowAnonymous]
    [ServiceFilter(typeof(HateoasAuthorFilterAttribute))]
    // [ProducesResponseType(404)]
    // [ProducesResponseType(200)]
    public async Task<ActionResult<AuthorDtoWithBooks>> Get(int id)
    {
        var author = await _context.Authors.Include(y => y.BooksAuthors).ThenInclude(z => z.Book)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (author == null) return NotFound();

        var dto = _mapper.Map<AuthorDtoWithBooks>(author);

        return dto;
    }

    [HttpGet("{name}", Name = "obtenerAutorPorNombrev1")]
    public async Task<ActionResult<List<AuthorDto>>> GetByName([FromRoute] string name)
    {
        var author = await _context.Authors.Where(x => x.Name.Contains(name)).ToListAsync();

        return Ok(_mapper.Map<List<AuthorDto>>(author));
    }

    [HttpPost(Name = "crearAutorv1")]
    public async Task<IActionResult> Post([FromBody] AuthorCreatedDto authorCreatedDto)
    {
        //Validaciones en el controlador
        var existsUser = await _context.Authors.AnyAsync(x => x.Name == authorCreatedDto.Name);

        if (existsUser) return BadRequest($"Ya existe un autor con el nombre {authorCreatedDto.Name}");

        var author = _mapper.Map<Author>(authorCreatedDto);

        _context.Add((object)author);
        await _context.SaveChangesAsync();

        var authorDto = _mapper.Map<AuthorDto>(author);
        return CreatedAtRoute("obtenerAutor", new { id = author.Id }, authorDto);
    }

    [HttpPut("{id:int}", Name = "actualizarAutorv1")] //Api/autores/id = 1 o 2
    public async Task<IActionResult> Put(AuthorCreatedDto authorDto, int id)
    {
        var exists = await _context.Authors.AnyAsync(x => x.Id == id);
        if (!exists) return NotFound();

        var author = _mapper.Map<Author>(authorDto);
        author.Id = id;

        _context.Update(author);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>
    ///     Borra un autor en la base de datos
    /// </summary>
    /// <param name="id"> ID del autor a borrar</param>
    /// <returns></returns>
    [HttpDelete("{id:int}", Name = "borrarAutorv1")] //Api/autores/2
    public async Task<IActionResult> Delete(int id)
    {
        var exists = await _context.Authors.AnyAsync(x => x.Id == id);
        if (!exists) return NotFound();

        _context.Remove(new Author { Id = id });
        await _context.SaveChangesAsync();
        return NoContent();
    }
}