namespace BibliotecaApi.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Rol
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
