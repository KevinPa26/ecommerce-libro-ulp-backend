using ecommerce_libro_ulp_backend.Models;

public interface ILibroRepository : IRepository<Libro>
{
	abstract Task<(List<Libro> Items, int TotalCount)> ObtenerTodos(string? titulo, string? autor, string? isbn, int? estado, int pageNumber, int pageSize);
}