using ecommerce_libro_ulp_backend.Models;

public interface IDetalleCompraRepository : IRepository<DetalleCompra>
{
	abstract List<DetalleCompra> ObtenerPorCompraId(int compra_id);
	abstract List<DetalleCompra> ObtenerPorLibroId(int libro_id);
}