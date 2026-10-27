using ecommerce_libro_ulp_backend.Models;

public interface ICompraRepository : IRepository<Compra>
{
	abstract Task<(List<Compra> Items, int TotalCount)> ObtenerTodos(DateOnly? fechaExacta, DateOnly? desde, DateOnly? hasta, int pageNumber, int pageSize, int? usuarioId = null);
}