using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Entities;

public class Book
{
    public int Id { get; set; }

    [Required]
    [FirstCapitalLetter]
    [StringLength(250)]
    public string Title { get; set; }

    public DateTime? PublicationDate { get; set; }

    public List<Comment> Comments { get; set; }
    public List<BookAuthor> BooksAuthors { get; set; }
}