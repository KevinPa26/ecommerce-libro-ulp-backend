using ecommerce_libro_ulp_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_libro_ulp_backend.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Libro> Libros => Set<Libro>();
    public DbSet<ImagenLibro> ImagenesLibro => Set<ImagenLibro>();
    public DbSet<Carrito> Carritos => Set<Carrito>();
    public DbSet<Favorito> Favoritos => Set<Favorito>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<DetalleCompra> DetallesCompra => Set<DetalleCompra>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .Property(u => u.Rol)
            .HasConversion<string>();

        modelBuilder.Entity<Usuario>()
            .Property(u => u.Estado)
            .HasConversion<string>();

        // 2. Entidad Libro (Estados del libro)
        modelBuilder.Entity<Libro>()
            .Property(l => l.Estado)
            .HasConversion<string>();
    }
}