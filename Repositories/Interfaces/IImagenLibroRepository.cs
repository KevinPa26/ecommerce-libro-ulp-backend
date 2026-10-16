using ecommerce_libro_ulp_backend.Models;

public interface IImagenLibroRepository : IRepository<ImagenLibro>
{
	abstract List<ImagenLibro> ObtenerPorLibroId(int libro_id);
	abstract ImagenLibro? ObtenerPorLibroIdPrincipal(int libro_id); // Para la imagen principal
}