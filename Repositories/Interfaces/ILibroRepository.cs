using ecommerce_libro_ulp_backend.Models;

public interface ILibroRepository : IRepository<Libro>
{
	abstract Task<List<Libro>> ObtenerPorISBN(string isbn);
	abstract Task<List<Libro>> ObtenerPorNombre(string nombre);
	abstract Task<List<Libro>> ObtenerPorAutor(string autor);
	abstract Task<List<Libro>> ObtenerPorVendedor(string vendedor);
	abstract Task<List<Libro>> ObtenerPorRangoDePrecios(decimal min, decimal max);
}