using ecommerce_libro_ulp_backend.Models;

public interface IDetalleCompraRepository : IRepository<DetalleCompra>
{
	abstract Task<List<DetalleCompra>> ObtenerPorCompra(int compra_id);
	abstract Task<List<DetalleCompra>> ObtenerPorCompra(Compra compra);
	abstract Task<List<DetalleCompra>> ObtenerPorLibro(int libro_id);
	abstract Task<List<DetalleCompra>> ObtenerPorLibro(Libro libro);
}