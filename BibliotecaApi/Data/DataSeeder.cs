namespace BibliotecaApi.Data;

using System.Linq;
using System.Threading.Tasks;
using BibliotecaApi.Models;
using Microsoft.AspNetCore.Identity;

public class DataSeeder
{
    private readonly ApplicationDbContext _context;

    public DataSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync()
    {
        // Roles iniciales
        if (!_context.Roles.Any())
        {
            var adminRol = new Rol { Nombre = "Admin" };
            var clientRol = new Rol { Nombre = "Cliente" };
            _context.Roles.AddRange(adminRol, clientRol);
            await _context.SaveChangesAsync();
        }

        // Usuario administrador inicial
        if (!_context.Usuarios.Any())
        {
            var adminRol = _context.Roles.First(r => r.Nombre == "Admin");
            var admin = new Usuario
            {
                Nombre = "Administrador",
                Email = "admin@uni.edu",
                RolId = adminRol.Id
            };
            var hasher = new PasswordHasher<Usuario>();
            admin.PasswordHash = hasher.HashPassword(admin, "Admin123!");
            _context.Usuarios.Add(admin);
            await _context.SaveChangesAsync();
        }

        // Autor y libro de prueba
        if (!_context.Autores.Any())
        {
            var autor = new Autor { Nombre = "Gabriel García Márquez", Bio = "Autor Colombiano" };
            _context.Autores.Add(autor);
            await _context.SaveChangesAsync();

            _context.Libros.Add(new Libro
            {
                Titulo = "Cien años de soledad",
                Descripcion = "Novela emblemática.",
                ISBN = "1234567890",
                Precio = 45.50m,
                AutorId = autor.Id,
                Disponible = true
            });

            await _context.SaveChangesAsync();
        }
    }
}
