using ecommerce_libro_ulp_backend.Models;

public interface ICompraRepository : IRepository<Compra>
{
	abstract Task<List<Compra>> ObtenerPorUsuarioId(int user_id);
	abstract Task<List<Compra>> ObtenerPorFecha(DateTime fecha);
	abstract Task<List<Compra>> ObtenerPorFecha(DateTime desde, DateTime hasta);
	abstract Task<List<Compra>> ObtenerPorFecha(DateTime desde, DateTime hasta, Usuario usuario);
	abstract Task<List<Compra>> ObtenerPorFecha(DateTime fecha, Usuario usuario);
}