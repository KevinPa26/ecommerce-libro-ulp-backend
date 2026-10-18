
using ecommerce_libro_ulp_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_libro_ulp_backend.Data;

public  class DataContext: DbContext
{
    public DataContext(DbContextOptions<DataContext> options): base(options) {}

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Libro> Libros { get; set; }
    public DbSet<ImagenLibro> ImagenesLibro { get; set; }
    public DbSet<Carrito> Carritos { get; set; }
    public DbSet<Favorito> Favoritos { get; set; }
    public DbSet<Compra> Compras { get; set; }
    public DbSet<DetalleCompra> DetallesCompra { get; set; }
}