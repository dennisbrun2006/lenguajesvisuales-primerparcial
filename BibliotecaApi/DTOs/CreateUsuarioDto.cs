namespace BibliotecaApi.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateUsuarioDto
{
    [Required, StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}
