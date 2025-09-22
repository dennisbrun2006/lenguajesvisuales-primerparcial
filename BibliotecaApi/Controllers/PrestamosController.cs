namespace BibliotecaApi.Controllers;

using BibliotecaApi.Data;
using BibliotecaApi.DTOs;
using BibliotecaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/prestamos")]
public class PrestamosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public PrestamosController(ApplicationDbContext context) => _context = context;

    // GET /api/prestamos?usuarioId=&libroId=
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? usuarioId = null, [FromQuery] int? libroId = null)
    {
        var q = _context.Prestamos
            .Include(p => p.Usuario)
            .Include(p => p.Libro)
            .AsQueryable();

        if (usuarioId.HasValue) q = q.Where(p => p.UsuarioId == usuarioId.Value);
        if (libroId.HasValue) q = q.Where(p => p.LibroId == libroId.Value);

        var data = await q
            .OrderByDescending(p => p.Id)
            .Select(p => new
            {
                p.Id,
                Usuario = new { p.UsuarioId, p.Usuario.Nombre, p.Usuario.Email },
                Libro = new { p.LibroId, p.Libro.Titulo },
                p.FechaPrestamo,
                p.FechaDevolucion,
                p.Activo
            })
            .ToListAsync();

        return Ok(new { success = true, data });
    }

    // POST /api/prestamos
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CreatePrestamoDto dto)
    {
        var user = await _context.Usuarios.FindAsync(dto.UsuarioId);
        if (user == null)
            return BadRequest(new { success = false, error = new { code = "INVALID_USER", message = "UsuarioId no existe" } });

        var libro = await _context.Libros.FindAsync(dto.LibroId);
        if (libro == null)
            return BadRequest(new { success = false, error = new { code = "INVALID_BOOK", message = "LibroId no existe" } });

        if (!libro.Disponible)
            return Conflict(new { success = false, error = new { code = "BOOK_NOT_AVAILABLE", message = "El libro no está disponible" } });

        var prestamo = new Prestamo
        {
            UsuarioId = dto.UsuarioId,
            LibroId = dto.LibroId,
            FechaPrestamo = DateTime.UtcNow,
            Activo = true
        };

        libro.Disponible = false;

        _context.Prestamos.Add(prestamo);
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Préstamo creado", data = new { prestamo.Id } });
    }

    // POST /api/prestamos/devolver
    [Authorize]
    [HttpPost("devolver")]
    public async Task<IActionResult> Devolver([FromBody] DevolverPrestamoDto dto)
    {
        var p = await _context.Prestamos.Include(x => x.Libro).FirstOrDefaultAsync(x => x.Id == dto.PrestamoId);
        if (p == null)
            return NotFound(new { success = false, error = new { code = "NOT_FOUND", message = "Préstamo no encontrado" } });

        if (!p.Activo)
            return Conflict(new { success = false, error = new { code = "ALREADY_RETURNED", message = "El préstamo ya fue devuelto" } });

        p.Activo = false;
        p.FechaDevolucion = DateTime.UtcNow;
        p.Libro.Disponible = true;

        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Libro devuelto" });
    }
}
