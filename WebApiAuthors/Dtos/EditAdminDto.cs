using System.ComponentModel.DataAnnotations;

namespace WebApiAuthors.Dtos;

public class EditAdminDto
{
    [Required] [EmailAddress] public string Email { get; set; }
}
