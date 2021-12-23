using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Dtos;

public class BookCreatedDto
{
    [FirstCapitalLetter]
    [StringLength(250)]
    public string Title { get; set; }

    public List<int> AuthorIds { get; set; }
}