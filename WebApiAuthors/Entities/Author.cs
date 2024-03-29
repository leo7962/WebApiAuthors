﻿using System.ComponentModel.DataAnnotations;
using WebApiAuthors.Validations;

namespace WebApiAuthors.Entities;

//Validaciones en el módelo
public class Author
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(120, ErrorMessage = "El campo {0} no debe tener más de {1} carácteres")]
    [FirstCapitalLetter]
    public string Name { get; set; }

    public List<BookAuthor> BooksAuthors { get; set; }
}