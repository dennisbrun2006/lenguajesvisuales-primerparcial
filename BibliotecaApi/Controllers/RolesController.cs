namespace BibliotecaApi.Controllers;

using BibliotecaApi.Data;
using BibliotecaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/roles")]
public class RolesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public RolesController(ApplicationDbContext context) => _context = context;

    // GET /api/roles
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _context.Roles
            .Select(r => new { r.Id, r.Nombre })
            .ToListAsync();

        return Ok(new { success = true, data = roles });
    }

    // GET /api/roles/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var rol = await _context.Roles.FindAsync(id);
        if (rol == null)
            return NotFound(new { success = false, error = new { code = "NOT_FOUND", message = "Rol no encontrado" } });

        return Ok(new { success = true, data = rol });
    }

    // POST /api/roles (solo admin debería hacerlo)
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Rol rol)
    {
        if (await _context.Roles.AnyAsync(r => r.Nombre == rol.Nombre))
            return Conflict(new { success = false, error = new { code = "ROLE_EXISTS", message = "El rol ya existe" } });

        _context.Roles.Add(rol);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = rol.Id }, new { success = true, data = rol });
    }
}
