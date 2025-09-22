namespace BibliotecaApi.Controllers;

using BibliotecaApi.Data;
using BibliotecaApi.DTOs;
using BibliotecaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/libros")]
public class LibrosController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public LibrosController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int limit = 10, [FromQuery] int? autorId = null)
    {
        var query = _context.Libros.Include(l => l.Autor).AsQueryable();
        if (autorId.HasValue) query = query.Where(l => l.AutorId == autorId.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(l => l.Id)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(l => new { l.Id, l.Titulo, l.Precio, l.Disponible, Autor = l.Autor.Nombre })
            .ToListAsync();

        return Ok(new
        {
            success = true,
            data = new
            {
                libros = items,
                pagination = new
                {
                    current_page = page,
                    per_page = limit,
                    total,
                    total_pages = (int)Math.Ceiling(total / (double)limit)
                }
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var libro = await _context.Libros.Include(l => l.Autor).FirstOrDefaultAsync(l => l.Id == id);
        if (libro == null)
            return NotFound(new { success = false, error = new { code = "NOT_FOUND", message = "Libro no encontrado" } });

        return Ok(new { success = true, data = libro });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLibroDto dto)
    {
        if (!await _context.Autores.AnyAsync(a => a.Id == dto.AutorId))
            return BadRequest(new { success = false, error = new { code = "INVALID_AUTHOR", message = "AutorId no existe" } });

        var libro = new Libro
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            ISBN = dto.ISBN,
            Precio = dto.Precio,
            AutorId = dto.AutorId,
            Disponible = true
        };

        _context.Libros.Add(libro);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = libro.Id },
            new { success = true, message = "Libro creado exitosamente", data = libro });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLibroDto dto)
    {
        var libro = await _context.Libros.FindAsync(id);
        if (libro == null)
            return NotFound(new { success = false, error = new { code = "NOT_FOUND", message = "Libro no encontrado" } });

        if (!await _context.Autores.AnyAsync(a => a.Id == dto.AutorId))
            return BadRequest(new { success = false, error = new { code = "INVALID_AUTHOR", message = "AutorId no existe" } });

        libro.Titulo = dto.Titulo;
        libro.Descripcion = dto.Descripcion;
        libro.ISBN = dto.ISBN;
        libro.Precio = dto.Precio;
        libro.AutorId = dto.AutorId;

        await _context.SaveChangesAsync();
        return Ok(new { success = true, message = "Libro actualizado exitosamente", data = libro });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var libro = await _context.Libros.FindAsync(id);
        if (libro == null)
            return NotFound(new { success = false, error = new { code = "NOT_FOUND", message = "Libro no encontrado" } });

        // Soft delete
        libro.Disponible = false;
        await _context.SaveChangesAsync();

        return Ok(new { success = true, message = "Libro eliminado exitosamente" });
    }
}
