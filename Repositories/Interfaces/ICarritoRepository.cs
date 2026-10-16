using ecommerce_libro_ulp_backend.Models;

public interface ICarritoRepository : IRepository<Carrito>
{
	abstract Task<List<Carrito>> ObtenerPorUsuarioId(int user_id);
	abstract Task<List<Carrito>> ObtenerPorUsuario(Usuario usuario);
}