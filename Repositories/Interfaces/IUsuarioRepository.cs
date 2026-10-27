using ecommerce_libro_ulp_backend.Models;

public interface IUsuarioRepository : IRepository<Usuario>
{
	abstract Task<List<Usuario>> ObtenerPorNombre(string nombre);
	abstract Task<List<Usuario>> ObtenerPorEstado(EstadoUsuario estado);
	abstract Task<List<Usuario>> ObtenerPorRol(RolUsuario rol);
	abstract Task<Usuario?> ObtenerPorEmail(string email);
	abstract Task<string?> ObtenerAvatarPorId(int id);
	Task<(List<Usuario> Items, int TotalCount)> ObtenerPaginados(string? busqueda, RolUsuario? rol, EstadoUsuario? estado, int pageNumber, int pageSize);

}