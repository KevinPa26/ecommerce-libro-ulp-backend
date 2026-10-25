using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ecommerce_libro_ulp_backend.Models
{
    public class Libro
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El vendedor es obligatorio.")]
        [Column("vendedor_id")]
        public int VendedorId { get; set; }

        [ForeignKey("VendedorId")]
        public Usuario? Vendedor { get; set; }

        [StringLength(13, MinimumLength = 10, ErrorMessage = "El ISBN13 debe tener entre 10 y 13 caracteres.")]
        public string? Isbn13 { get; set; }

        [Required(ErrorMessage = "El título del libro es obligatorio.")]
        [StringLength(150, ErrorMessage = "El título no puede superar los 150 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El autor es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del autor no puede superar los 100 caracteres.")]
        public string Autor { get; set; } = string.Empty;

        [Column("descripcion_vendedor")]
        public string? DescripcionVendedor { get; set; }

        [Column("descripcion_libro")]
        public string? DescripcionLibro { get; set; }

        [Required(ErrorMessage = "El precio es obligatorio.")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser un valor negativo.")]
        public int Stock { get; set; } = 0;

        [Required(ErrorMessage = "El estado del libro es obligatorio.")]
        public EstadoLibro Estado { get; set; } = EstadoLibro.DISPONIBLE;

        [Column("usuario_creador")]
        public int? UsuarioCreador { get; set; }

        [Column("usuario_delete")]
        public int? UsuarioDelete { get; set; }

        // Relaciones
        public ICollection<ImagenLibro> Imagenes { get; set; } = new List<ImagenLibro>();
        [JsonIgnore]
        public ICollection<Carrito> Carritos { get; set; } = new List<Carrito>();
        [JsonIgnore]
        public ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
        [JsonIgnore]
        public ICollection<DetalleCompra> DetallesCompra { get; set; } = new List<DetalleCompra>();
    }
}