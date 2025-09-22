namespace BibliotecaApi.Models;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Usuario
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    public int RolId { get; set; }
    public Rol? Rol { get; set; }

    public ICollection<Prestamo> Prestamos { get; set; } = new List<Prestamo>();
}
