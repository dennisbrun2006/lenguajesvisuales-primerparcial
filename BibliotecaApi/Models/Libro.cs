namespace BibliotecaApi.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Libro
{
    public int Id { get; set; }

    [Required, StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Descripcion { get; set; }

    [StringLength(20)]
    public string? ISBN { get; set; }

    // La precisión decimal se configura en el DbContext (OnModelCreating)
    public decimal Precio { get; set; }

    public int AutorId { get; set; }
    public Autor? Autor { get; set; }

    public bool Disponible { get; set; } = true;

    public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
