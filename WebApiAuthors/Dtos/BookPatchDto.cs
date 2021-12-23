using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Dtos;

public class BookPatchDto
{
    [FirstCapitalLetter]
    [StringLength(250)]
    [Required]
    public string Title { get; set; }

    public DateTime PublicationDate { get; set; }
}