using ecommerce_libro_ulp_backend.Models;

public interface ILibroRepository : IRepository<Libro>
{
	abstract List<Libro> ObtenerPorISBN(string isbn);
	abstract List<Libro> ObtenerPorNombre(string nombre);
	abstract List<Libro> ObtenerPorAutor(string autor);
	abstract List<Libro> ObtenerPorVendedor(string vendedor);
	abstract List<Libro> ObtenerPorRangoDePrecios(decimal min, decimal max);
}