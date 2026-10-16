using ecommerce_libro_ulp_backend.Models;

public interface IUsuarioRepository : IRepository<Usuario>
{
	abstract List<Usuario> ObtenerPorNombre(string nombre);
	abstract List<Usuario> ObtenerPorEstado(string estado);
	abstract List<Usuario> ObtenerPorRol(string rol);
	abstract Usuario? ObtenerPorEmail(string email);

}