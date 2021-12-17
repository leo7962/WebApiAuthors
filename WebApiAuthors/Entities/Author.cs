using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Entities;

//Validaciones en el módelo
public class Author : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(120, ErrorMessage = "El campo {0} no debe tener más de {1} carácteres")]
    //[FirstCapitalLetter]
    public string Name { get; set; }

    /*[Range(18, 120)] [NotMapped] public int Age { get; set; }
    [CreditCard] [NotMapped] public string CreditCard { get; set; }
    [Url] [NotMapped] public string URL { get; set; }*/
    public List<Book> Books { get; set; }

    /*public int less { get; set; }
    public int big { get; set; }*/

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(Name))
        {
            var firstLetter = Name[0].ToString();

            if (firstLetter != firstLetter.ToUpper())
                yield return new ValidationResult("la primera letra debe ser mayúscula", new[] {nameof(Name)});
        }

        /*if (less > big)
        {
            yield return new ValidationResult("Este valor no puede ser más grande que el campo mayor",
                new[] {nameof(less)});
        }*/
    }
}
