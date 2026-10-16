using ecommerce_libro_ulp_backend.Models;

public interface IUsuarioRepository : IRepository<Usuario>
{
	abstract Task<List<Usuario>> ObtenerPorNombre(string nombre);
	abstract Task<List<Usuario>> ObtenerPorEstado(EstadoUsuario estado);
	abstract Task<List<Usuario>> ObtenerPorRol(RolUsuario rol);
	abstract Task<Usuario?> ObtenerPorEmail(string email);

}