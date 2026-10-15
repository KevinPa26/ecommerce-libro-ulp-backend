
using ecommerce_libro_ulp_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace ecommerce_libro_ulp_backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

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

        // USUARIOS
        
        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("usuarios");

            e.HasKey(x => x.Id);

            e.Property(x => x.Id)
                .HasColumnName("id");

            e.Property(x => x.Nombre)
                .HasColumnName("nombre")
                .HasMaxLength(100)
                .IsRequired();

            e.Property(x => x.Avatar)
                .HasColumnName("avatar")
                .HasMaxLength(255)
                .IsRequired(false);

            e.Property(x => x.Rol)
                .HasColumnName("rol")
                .HasColumnType("enum('ADMIN','USUARIO')")
                .HasConversion<string>()
                .IsRequired();

            e.Property(x => x.Email)
                .HasColumnName("email")
                .HasMaxLength(150)
                .IsRequired();

            e.HasIndex(x => x.Email)
                .IsUnique();

            e.Property(x => x.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(255)
                .IsRequired();

            e.Ignore(x => x.Password);
            e.Ignore(x => x.RolNombre);

            e.Property(x => x.FechaRegistro)
                .HasColumnName("fecha_registro")
                .HasColumnType("timestamp")
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAdd();

            e.Property(x => x.Activo)
                .HasColumnName("estado")
                .HasColumnType("enum('ACTIVO','INACTIVO','SUSPENDIDO')")
                .HasConversion<string>()
                .IsRequired();

            e.Property(x => x.UsuarioCreador)
                .HasColumnName("usuario_creador");

            e.Property(x => x.UsuarioDelete)
                .HasColumnName("usuario_delete");

            e.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioCreador)
                .HasConstraintName("fk_usuario_creador")
                .OnDelete(DeleteBehavior.SetNull);

            e.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioDelete)
                .HasConstraintName("fk_usuario_delete")
                .OnDelete(DeleteBehavior.SetNull);
        });


        // LIBROS
        modelBuilder.Entity<Libro>(e =>
        {
            e.ToTable("libros");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.VendedorId).HasColumnName("vendedor_id");
            e.Property(x => x.Isbn13).HasColumnName("isbn13").HasMaxLength(13);
            e.Property(x => x.Titulo).HasColumnName("titulo")
                .HasMaxLength(150).IsRequired();
            e.Property(x => x.Autor).HasColumnName("autor")
                .HasMaxLength(100).IsRequired();
            e.Property(x => x.DescripcionVendedor).HasColumnName("descripcion_vendedor")
                .HasColumnType("text");
            e.Property(x => x.DescripcionLibro).HasColumnName("descripcion_libro")
                .HasColumnType("text");
            e.Property(x => x.Precio).HasColumnName("precio")
                .HasPrecision(10, 2).IsRequired();
            e.Property(x => x.Stock).HasColumnName("stock")
                .HasDefaultValue(0);
            e.Property(x => x.Estado)
                .HasColumnName("estado")
                .HasColumnType("enum('DISPONIBLE','AGOTADO','INACTIVO')")
                .HasConversion<string>()
                .IsRequired();
            e.Property(x => x.UsuarioCreador).HasColumnName("usuario_creador");
            e.Property(x => x.UsuarioDelete).HasColumnName("usuario_delete");

            e.HasIndex(x => x.Titulo).HasDatabaseName("idx_titulo");
            e.HasIndex(x => x.Autor).HasDatabaseName("idx_autor");

            e.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.VendedorId)
                .HasConstraintName("fk_libros_vendedor")
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioCreador)
                .HasConstraintName("fk_libros_creador")
                .OnDelete(DeleteBehavior.SetNull);

            e.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioDelete)
                .HasConstraintName("fk_libros_delete")
                .OnDelete(DeleteBehavior.SetNull);
        });

        // IMAGEN_LIBRO
        modelBuilder.Entity<ImagenLibro>(e =>
        {
            e.ToTable("imagen_libro");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.LibroId).HasColumnName("libro_id");
            e.Property(x => x.Path).HasColumnName("path")
                .HasMaxLength(255).IsRequired();
            e.Property(x => x.EsPrincipal).HasColumnName("es_principal")
                .HasDefaultValue(false);

            e.HasOne<Libro>()
                .WithMany()
                .HasForeignKey(x => x.LibroId)
                .HasConstraintName("fk_imagen_libro")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CARRITO
        modelBuilder.Entity<Carrito>(e =>
        {
            e.ToTable("carrito");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UsuarioId).HasColumnName("usuario_id");
            e.Property(x => x.LibroId).HasColumnName("libro_id");
            e.Property(x => x.Cantidad).HasColumnName("cantidad")
                .HasDefaultValue(1);
            e.Property(x => x.FechaAgregado).HasColumnName("fecha_agregado")
                .HasColumnType("date");

            e.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .HasConstraintName("fk_carrito_usuario")
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne<Libro>()
                .WithMany()
                .HasForeignKey(x => x.LibroId)
                .HasConstraintName("fk_carrito_libro")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // FAVORITO
        modelBuilder.Entity<Favorito>(e =>
        {
            e.ToTable("favorito");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UsuarioId).HasColumnName("usuario_id");
            e.Property(x => x.LibroId).HasColumnName("libro_id");
            e.Property(x => x.FechaAgregado).HasColumnName("fecha_agregado")
                .HasColumnType("date");

            e.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .HasConstraintName("fk_favorito_usuario")
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne<Libro>()
                .WithMany()
                .HasForeignKey(x => x.LibroId)
                .HasConstraintName("fk_favorito_libro")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // COMPRAS
        modelBuilder.Entity<Compra>(e =>
        {
            e.ToTable("compras");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.UsuarioId).HasColumnName("usuario_id");
            e.Property(x => x.FechaCompra).HasColumnName("fecha_compra")
                .HasColumnType("date").IsRequired();

            e.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .HasConstraintName("fk_compras_usuario")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // DETALLE_COMPRAS
        modelBuilder.Entity<DetalleCompra>(e =>
        {
            e.ToTable("detalle_compras");
            e.HasKey(x => x.Id);

            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.CompraId).HasColumnName("compra_id");
            e.Property(x => x.LibroId).HasColumnName("libro_id");
            e.Property(x => x.Cantidad).HasColumnName("cantidad");
            e.Property(x => x.PrecioUnitario).HasColumnName("precio_unitario")
                .HasPrecision(10, 2).IsRequired();
            e.Property(x => x.Puntos).HasColumnName("puntos")
                .HasDefaultValue(0);

            e.HasOne<Compra>()
                .WithMany()
                .HasForeignKey(x => x.CompraId)
                .HasConstraintName("fk_detalle_compra")
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne<Libro>()
                .WithMany()
                .HasForeignKey(x => x.LibroId)
                .HasConstraintName("fk_detalle_libro")
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
