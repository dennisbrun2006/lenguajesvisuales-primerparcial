namespace BibliotecaApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class DevolverPrestamoDto
{
    [Required] public int PrestamoId { get; set; }
}
