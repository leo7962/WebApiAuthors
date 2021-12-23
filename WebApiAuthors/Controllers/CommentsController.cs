using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Context;
using WebApiAuthors.Dtos;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers;

[ApiController]
[Route("api/libros/{bookId:int}/comentarios")]
public class CommentsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public CommentsController(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<CommentDto>>> Get(int bookId)
    {
        var exists = await _context.Books.AnyAsync(x => x.Id == bookId);
        if (!exists) return NotFound();
        var comments = await _context.Comments.Where(x => x.BookId == bookId).ToListAsync();
        return Ok(_mapper.Map<List<CommentDto>>(comments));
    }

    [HttpGet("{id:int}", Name = "ObtenerComentario")]
    public async Task<ActionResult<CommentDto>> GetId(int id)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
        if (comment == null) return NotFound();

        return Ok(_mapper.Map<CommentDto>(comment));
    }

    [HttpPost]
    public async Task<ActionResult> Post(int bookId, CommentCreatedDto commentCreatedDto)
    {
        var exists = await _context.Books.AnyAsync(x => x.Id == bookId);
        if (!exists) return NotFound();

        var comment = _mapper.Map<Comment>(commentCreatedDto);
        comment.BookId = bookId;

        _context.Add(comment);
        await _context.SaveChangesAsync();

        var commentDto = _mapper.Map<CommentDto>(comment);

        return CreatedAtRoute("ObtenerComentario", new {id = comment.Id, bookId}, commentDto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int bookId, int id, CommentCreatedDto commentCreatedDto)
    {
        var exists = await _context.Books.AnyAsync(x => x.Id == bookId);
        if (!exists) return NotFound();

        var existsComment = await _context.Comments.AnyAsync(x => x.Id == id);
        if (!existsComment) return NotFound();

        var comment = _mapper.Map<Comment>(commentCreatedDto);
        comment.Id = id;
        comment.BookId = bookId;
        _context.Update(comment);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}