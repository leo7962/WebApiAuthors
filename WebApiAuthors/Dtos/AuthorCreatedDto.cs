using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Dtos;

public class AuthorCreatedDto
{
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(120, ErrorMessage = "El campo {0} no debe tener más de {1} carácteres")]
    [FirstCapitalLetter]
    public string Name { get; set; }
}