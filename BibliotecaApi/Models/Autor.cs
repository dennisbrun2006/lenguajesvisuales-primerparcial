namespace BibliotecaApi.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Autor
{
    public int Id { get; set; }

    [Required, StringLength(150)]
    public string Nombre { get; set; } = string.Empty;

    public string? Bio { get; set; }

    public ICollection<Libro> Libros { get; set; } = new List<Libro>();
}
