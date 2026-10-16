using ecommerce_libro_ulp_backend.Models;

public interface IUsuarioRepository : IRepository<Usuario>
{
	abstract Task<List<Usuario>> ObtenerPorNombre(string nombre);
	abstract Task<List<Usuario>> ObtenerPorEstado(string estado);
	abstract Task<List<Usuario>> ObtenerPorRol(string rol);
	abstract Task<Usuario?> ObtenerPorEmail(string email);

}