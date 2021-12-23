using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Dtos;

public class BookDto
{
    public int Id { get; set; }

    [FirstCapitalLetter]
    [StringLength(250)]
    public string Title { get; set; }

    //public List<CommentDto> Comments { get; set; }
}
