using ecommerce_libro_ulp_backend.Models;

public interface ICarritoRepository : IRepository<Carrito>
{
	abstract Task<Carrito?> ObtenerPorUsuarioId(int user_id);
}