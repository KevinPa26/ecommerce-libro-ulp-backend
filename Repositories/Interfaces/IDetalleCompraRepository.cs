using ecommerce_libro_ulp_backend.Models;

public interface IDetalleCompraRepository : IRepository<DetalleCompra>
{
	abstract Task<List<DetalleCompra>> ObtenerPorCompraId(int compra_id);
	abstract Task<List<DetalleCompra>> ObtenerPorLibroId(int libro_id);
}