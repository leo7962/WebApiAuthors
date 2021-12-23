using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Context;
using WebApiAuthors.Dtos;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers;

[ApiController]
[Route("api/libros")]
public class BooksController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public BooksController(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<BookDtoWithAuthors>> Get(int id)
    {
        var book = await _context.Books.Include(y => y.BooksAuthors).ThenInclude(z => z.Author)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (book != null)
        {
            book.BooksAuthors = book.BooksAuthors.OrderBy(x => x.Order).ToList();
            return Ok(_mapper.Map<BookDtoWithAuthors>(book));
        }

        return BadRequest();
    }

    [HttpPost]
    public async Task<ActionResult> Post(BookCreatedDto bookCreatedDto)
    {
        if (bookCreatedDto.AuthorIds == null) return BadRequest("No se puede crear un libro sin autores");

        var authorsIds = await _context.Authors.Where(x => bookCreatedDto.AuthorIds.Contains(x.Id)).Select(y => y.Id)
            .ToListAsync();

        if (bookCreatedDto.AuthorIds.Count != authorsIds.Count)
            return BadRequest("No existe uno de los autores enviados");

        var book = _mapper.Map<Book>(bookCreatedDto);

        if (book.BooksAuthors != null)
            for (var i = 0; i < book.BooksAuthors.Count; i++)
                book.BooksAuthors[i].Order = i;

        _context.Add(book);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
