using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_libro_ulp_backend.Models
{
    [Table("Favorito")]
    public class Favorito
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }

        [Required(ErrorMessage = "El libro es obligatorio.")]
        [Column("libro_id")]
        public int LibroId { get; set; }

        [ForeignKey("LibroId")]
        public Libro? Libro { get; set; }

        [Column("fecha_agregado", TypeName = "date")]
        public DateTime? FechaAgregado { get; set; }
    }
}