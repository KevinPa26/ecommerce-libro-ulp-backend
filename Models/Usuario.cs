using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ecommerce_libro_ulp_backend.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "La ruta del avatar no puede superar los 255 caracteres.")]
        public string? Avatar { get; set; }

        [Required(ErrorMessage = "El rol del usuario es obligatorio.")]
        public RolUsuario Rol { get; set; } = RolUsuario.USUARIO;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(150, ErrorMessage = "El email no puede superar los 150 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Column("password_hash")]
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;

        [NotMapped] // No se guarda en la BD, se usa para los formularios
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres.")]
        public string? Password { get; set; }

        [NotMapped]
        public string RolNombre => Rol.ToString();

        [Column("fecha_registro", TypeName = "timestamp")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Display(Name = "Activo")]
        [Required(ErrorMessage = "El estado del usuario es obligatorio.")]
        public EstadoUsuario Estado { get; set; } = EstadoUsuario.ACTIVO;

        [Column("usuario_creador")]
        public int? UsuarioCreador { get; set; }

        [Column("usuario_delete")]
        public int? UsuarioDelete { get; set; }

        [JsonIgnore]
        public ICollection<Libro> LibrosVendidos { get; set; } = new List<Libro>();
        [JsonIgnore]
        public ICollection<Carrito> Carritos { get; set; } = new List<Carrito>();
        [JsonIgnore]
        public ICollection<Favorito> Favoritos { get; set; } = new List<Favorito>();
        [JsonIgnore]
        public ICollection<Compra> Compras { get; set; } = new List<Compra>();
    }
}