namespace BibliotecaApi.Controllers;

using BibliotecaApi.Data;
using BibliotecaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/autores")]
public class AutoresController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public AutoresController(ApplicationDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var autores = await _context.Autores
            .OrderBy(a => a.Id)
            .Select(a => new { a.Id, a.Nombre, a.Bio })
            .ToListAsync();

        return Ok(new { success = true, data = autores });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var autor = await _context.Autores.FindAsync(id);
        if (autor == null)
            return NotFound(new { success = false, error = new { code = "NOT_FOUND", message = "Autor no encontrado" } });

        return Ok(new { success = true, data = autor });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Autor input)
    {
        if (string.IsNullOrWhiteSpace(input.Nombre))
            return BadRequest(new { success = false, error = new { code = "VALIDATION_ERROR", message = "Nombre es requerido" } });

        var autor = new Autor { Nombre = input.Nombre, Bio = input.Bio };
        _context.Autores.Add(autor);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = autor.Id }, new { success = true, data = autor });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Autor input)
    {
        var autor = await _context.Autores.FindAsync(id);
        if (autor == null)
            return NotFound(new { success = false, error = new { code = "NOT_FOUND", message = "Autor no encontrado" } });

        autor.Nombre = input.Nombre;
        autor.Bio = input.Bio;
        await _context.SaveChangesAsync();
        return Ok(new { success = true, data = autor });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var autor = await _context.Autores.FindAsync(id);
        if (autor == null)
            return NotFound(new { success = false, error = new { code = "NOT_FOUND", message = "Autor no encontrado" } });

        _context.Autores.Remove(autor);
        await _context.SaveChangesAsync();
        return Ok(new { success = true, message = "Autor eliminado" });
    }
}
