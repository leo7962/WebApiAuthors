using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Entities;

public class Author
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(10, ErrorMessage = "El campo {0} no debe tener más de {1} carácteres")]
    [FirstCapitalLetter]
    public string Name { get; set; }

    [Range(18, 120)] [NotMapped] public int Age { get; set; }
    [CreditCard] [NotMapped] public string CreditCard { get; set; }
    [Url] [NotMapped] public string URL { get; set; }
    public List<Book> Books { get; set; }
}
