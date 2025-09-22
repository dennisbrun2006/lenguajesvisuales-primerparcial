namespace BibliotecaApi.Controllers;

using BibliotecaApi.Data;
using BibliotecaApi.DTOs;
using BibliotecaApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public UsuariosController(ApplicationDbContext context) => _context = context;

    /// GET /api/usuarios
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var usuarios = await _context.Usuarios
            .Include(u => u.Rol)
            .Select(u => new { u.Id, u.Nombre, u.Email, Rol = u.Rol!.Nombre })
            .ToListAsync();

        return Ok(new { success = true, data = usuarios });
    }

    /// GET /api/usuarios/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound(new { success = false, error = new { code = "NOT_FOUND", message = "Usuario no encontrado" } });

        return Ok(new { success = true, data = new { user.Id, user.Nombre, user.Email, Rol = user.Rol!.Nombre } });
    }

    /// POST /api/usuarios/registro
    [HttpPost("registro")]
    public async Task<IActionResult> Register([FromBody] CreateUsuarioDto dto)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email))
            return Conflict(new { success = false, error = new { code = "EMAIL_TAKEN", message = "El email ya está registrado" } });

        var rolCliente = await _context.Roles.FirstAsync(r => r.Nombre == "Cliente");

        var user = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            RolId = rolCliente.Id
        };

        var hasher = new PasswordHasher<Usuario>();
        user.PasswordHash = hasher.HashPassword(user, dto.Password);

        _context.Usuarios.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id },
            new { success = true, data = new { user.Id, user.Nombre, user.Email } });
    }
    [Authorize(Roles = "Admin")]
    [HttpPost("{id}/rol")]
    public async Task<IActionResult> AsignarRol(int id, [FromBody] AsignarRolDto dto)
    {
        var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
            return NotFound(new { success = false, error = new { code = "NOT_FOUND", message = "Usuario no encontrado" } });

        // Resolver el rol por Id o por Nombre
        Models.Rol? rol = null;

        if (dto.RolId.HasValue)
        {
            rol = await _context.Roles.FirstOrDefaultAsync(r => r.Id == dto.RolId.Value);
            if (rol == null)
                return BadRequest(new { success = false, error = new { code = "INVALID_ROLE", message = "RolId no existe" } });
        }
        else if (!string.IsNullOrWhiteSpace(dto.RolNombre))
        {
            rol = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == dto.RolNombre);
            if (rol == null)
                return BadRequest(new { success = false, error = new { code = "INVALID_ROLE", message = "RolNombre no existe" } });
        }
        else
        {
            return BadRequest(new { success = false, error = new { code = "ROLE_REQUIRED", message = "Debes enviar RolId o RolNombre" } });
        }

        user.RolId = rol!.Id;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            success = true,
            message = "Rol asignado correctamente",
            data = new { user.Id, user.Nombre, user.Email, Rol = rol.Nombre }
        });
    }
}
