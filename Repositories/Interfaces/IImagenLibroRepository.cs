using ecommerce_libro_ulp_backend.Models;

public interface IImagenLibroRepository : IRepository<ImagenLibro>
{
	abstract Task<List<ImagenLibro>> ObtenerPorLibro(int libro_id);
	abstract Task<List<ImagenLibro>> ObtenerPorLibro(Libro libro);
	abstract Task<ImagenLibro?> ObtenerImagenPrincipal(int libro_id); // Para la imagen principal
	abstract Task<ImagenLibro?> ObtenerImagenPrincipal(Libro libro);
}