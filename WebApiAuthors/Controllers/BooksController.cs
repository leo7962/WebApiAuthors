using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Context;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers;

[ApiController]
[Route("api/libros")]
public class BooksController : ControllerBase
{
    private readonly DataContext _context;

    public BooksController(DataContext context)
    {
        _context = context;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Book>> Get(int id)
    {
        return await _context.Books.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Book book)
    {
        var existsAuhtor = await _context.Authors.AnyAsync(x => x.Id == book.AuthorId);
        if (!existsAuhtor) return BadRequest($"No existe el author de Id: {book.AuthorId}");

        _context.Add(book);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
