using ecommerce_libro_ulp_backend.Models;

public interface ICompraRepository : IRepository<Compra>
{
	abstract List<Compra> ObtenerPorUsuarioId(int user_id);
	abstract List<Compra> ObtenerPorFecha(DateTime fecha);
	abstract List<Compra> ObtenerPorFecha(DateTime desde, DateTime hasta);
}