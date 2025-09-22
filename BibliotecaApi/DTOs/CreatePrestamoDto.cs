namespace BibliotecaApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreatePrestamoDto
{
    [Required] public int UsuarioId { get; set; }
    [Required] public int LibroId { get; set; }
}
