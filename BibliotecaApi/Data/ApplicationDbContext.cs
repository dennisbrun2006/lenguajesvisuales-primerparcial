namespace BibliotecaApi.Data;

using BibliotecaApi.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Autor> Autores => Set<Autor>();
    public DbSet<Libro> Libros => Set<Libro>();
    public DbSet<Prestamo> Prestamos => Set<Prestamo>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Precisión decimal para el campo Precio de Libro
        builder.Entity<Libro>()
               .Property(l => l.Precio)
               .HasColumnType("decimal(10,2)");

        // Usuario -> Rol (1:N) con delete restrict
        builder.Entity<Usuario>()
            .HasOne(u => u.Rol)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.RolId)
            .OnDelete(DeleteBehavior.Restrict);

        // Libro -> Autor (1:N)
        builder.Entity<Libro>()
            .HasOne(l => l.Autor)
            .WithMany(a => a.Libros)
            .HasForeignKey(l => l.AutorId);

        // Prestamo -> Usuario (N:1)
        builder.Entity<Prestamo>()
            .HasOne(p => p.Usuario)
            .WithMany(u => u.Prestamos)
            .HasForeignKey(p => p.UsuarioId);

        // Prestamo -> Libro (N:1)
        builder.Entity<Prestamo>()
            .HasOne(p => p.Libro)
            .WithMany(l => l.Prestamos)
            .HasForeignKey(p => p.LibroId);
    }
}
