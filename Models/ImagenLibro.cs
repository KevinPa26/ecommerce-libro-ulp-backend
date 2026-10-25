using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ecommerce_libro_ulp_backend.Models
{
    [Table("imagen_libro")]
    public class ImagenLibro
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe asociar la imagen a un libro.")]
        [Column("libro_id")]
        public int LibroId { get; set; }

        [ForeignKey("LibroId")]
        [JsonIgnore]
        public Libro? Libro { get; set; }

        [Required(ErrorMessage = "La ruta de la imagen es obligatoria.")]
        [StringLength(255, ErrorMessage = "La ruta no puede superar los 255 caracteres.")]
        public string Path { get; set; } = string.Empty;

        [Column("es_principal")]
        public bool EsPrincipal { get; set; } = false;
    }
}