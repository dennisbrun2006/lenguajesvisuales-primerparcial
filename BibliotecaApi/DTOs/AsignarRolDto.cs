namespace BibliotecaApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class AsignarRolDto
{
    // Podés enviar cualquiera de los dos (prioriza RolId si vienen ambos)
    public int? RolId { get; set; }

    [StringLength(50)]
    public string? RolNombre { get; set; }
}
