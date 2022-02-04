using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAuthors.Context;
using WebApiAuthors.Dtos;
using WebApiAuthors.Entities;

namespace WebApiAuthors.Controllers.V1;

[ApiController]
[Route("api/v1/libros/{bookId:int}/comentarios")]
public class CommentsController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<IdentityUser> _userManager;

    public CommentsController(DataContext context, IMapper mapper, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpGet(Name = "obtenerComentariosLibro")]
    public async Task<ActionResult<List<CommentDto>>> Get(int bookId)
    {
        var exists = await _context.Books.AnyAsync(x => x.Id == bookId);
        if (!exists) return NotFound();
        var comments = await _context.Comments.Where(x => x.BookId == bookId).ToListAsync();
        return Ok(_mapper.Map<List<CommentDto>>(comments));
    }

    [HttpGet("{id:int}", Name = "obtenerComentario")]
    public async Task<ActionResult<CommentDto>> GetId(int id)
    {
        var comment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);
        if (comment == null) return NotFound();

        return Ok(_mapper.Map<CommentDto>(comment));
    }

    [HttpPost(Name = "crearComentario")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult> Post(int bookId, CommentCreatedDto commentCreatedDto)
    {
        var emailClaim = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "email");

        if (emailClaim != null)
        {
            var email = emailClaim.Value;
            var user = await _userManager.FindByEmailAsync(email);
            var userId = user.Id;

            var exists = await _context.Books.AnyAsync(x => x.Id == bookId);

            if (!exists) return NotFound();

            var comment = _mapper.Map<Comment>(commentCreatedDto);
            comment.BookId = bookId;
            comment.UserId = userId;

            _context.Add(comment);
            await _context.SaveChangesAsync();

            var commentDto = _mapper.Map<CommentDto>(comment);

            return CreatedAtRoute("ObtenerComentario", new {id = comment.Id, bookId}, commentDto);
        }

        return BadRequest("El email no se encuentra registrado");
    }

    [HttpPut("{id:int}", Name = "actualizarComentario")]
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
