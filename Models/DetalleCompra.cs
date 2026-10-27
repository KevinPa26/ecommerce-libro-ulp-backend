using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_libro_ulp_backend.Models
{
    [Table("detalle_compras")]
    public class DetalleCompra
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "La compra asociada es obligatoria.")]
        [Column("compra_id")]
        public int CompraId { get; set; }

        [ForeignKey("CompraId")]
        public Compra? Compra { get; set; }

        [Required(ErrorMessage = "El libro es obligatorio.")]
        [Column("libro_id")]
        public int LibroId { get; set; }

        [ForeignKey("LibroId")]
        public Libro? Libro { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El precio unitario es obligatorio.")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio unitario debe ser mayor a 0.")]
        [Column("precio_unitario", TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        public int Puntos { get; set; } = 0;
    }
}