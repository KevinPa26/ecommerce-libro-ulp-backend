using ecommerce_libro_ulp_backend.Models;

public interface IImagenLibroRepository : IRepository<ImagenLibro>
{
	abstract Task<List<ImagenLibro>> ObtenerPorLibroId(int libro_id);
	abstract Task<ImagenLibro?> ObtenerPorLibroIdPrincipal(int libro_id); // Para la imagen principal
}