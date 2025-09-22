namespace BibliotecaApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateLibroDto
{
    [Required]
    public string Titulo { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public string? ISBN { get; set; }

    public decimal Precio { get; set; }

    [Required]
    public int AutorId { get; set; }
}
