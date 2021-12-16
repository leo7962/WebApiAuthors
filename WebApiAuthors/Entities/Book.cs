using WebApiAuthors.Validations;

namespace WebApiAuthors.Entities;

public class Book
{
    public int Id { get; set; }
    [FirstCapitalLetter] public string Title { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; }
}
