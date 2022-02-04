using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Context;
using WebApiAuthors.Dtos;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers.V1;

[ApiController]
[Route("api/v1/libros")]
public class BooksController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public BooksController(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("{id:int}", Name = "obtenerLibtro")]
    public async Task<ActionResult<BookDtoWithAuthors>> Get(int id)
    {
        var book = await _context.Books.Include(y => y.BooksAuthors).ThenInclude(z => z.Author)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (book == null) return NotFound();

        book.BooksAuthors = book.BooksAuthors.OrderBy(x => x.Order).ToList();
        return Ok(_mapper.Map<BookDtoWithAuthors>(book));
    }

    [HttpPost(Name = "crearLibro")]
    public async Task<ActionResult> Post(BookCreatedDto bookCreatedDto)
    {
        if (bookCreatedDto.AuthorIds == null) return BadRequest("No se puede crear un libro sin autores");

        var authorsIds = await _context.Authors.Where(x => bookCreatedDto.AuthorIds.Contains(x.Id)).Select(y => y.Id)
            .ToListAsync();

        if (bookCreatedDto.AuthorIds.Count != authorsIds.Count)
            return BadRequest("No existe uno de los autores enviados");

        var book = _mapper.Map<Book>(bookCreatedDto);

        AssignOrderAuthors(book);

        _context.Add(book);
        await _context.SaveChangesAsync();

        var bookDto = _mapper.Map<BookDto>(book);

        return CreatedAtRoute("ObtenerLibtro", new {id = book.Id}, bookDto);
    }

    [HttpPut("{id:int}", Name = "actualizarLibro")]
    public async Task<ActionResult> Put(int id, BookCreatedDto bookCreatedDto)
    {
        var book = await _context.Books.Include(x => x.BooksAuthors).FirstOrDefaultAsync(x => x.Id == id);

        if (book == null) return NotFound();

        book = _mapper.Map(bookCreatedDto, book);
        AssignOrderAuthors(book);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("{id:int}", Name = "patchLibro")]
    public async Task<ActionResult> Patch(int id, JsonPatchDocument<BookPatchDto> patchDocument)
    {
        if (patchDocument == null) return BadRequest();

        var book = await _context.Books.FirstOrDefaultAsync(x => x.Id == id);

        if (book == null) return NotFound();

        var bookDto = _mapper.Map<BookPatchDto>(book);

        patchDocument.ApplyTo(bookDto, ModelState);
        var valid = TryValidateModel(bookDto);

        if (!valid) return BadRequest(ModelState);

        _mapper.Map(bookDto, book);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}", Name = "borrarLibro")]
    public async Task<ActionResult> Delete(int id)
    {
        var exists = await _context.Books.AnyAsync(x => x.Id == id);
        if (!exists) return NotFound();

        _context.Remove(new Book {Id = id});
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private void AssignOrderAuthors(Book book)
    {
        if (book.BooksAuthors != null)
            for (var i = 0; i < book.BooksAuthors.Count; i++)
                book.BooksAuthors[i].Order = i;
    }
}
