using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_libro_ulp_backend.Models
{
    [Table("Compras")]
    public class Compra
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario comprador es obligatorio.")]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Required(ErrorMessage = "La fecha de compra es obligatoria.")]
        [Column("fecha_compra", TypeName = "date")]
        public DateOnly FechaCompra { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public ICollection<DetalleCompra> Detalles { get; set; } = new List<DetalleCompra>();
    }
}