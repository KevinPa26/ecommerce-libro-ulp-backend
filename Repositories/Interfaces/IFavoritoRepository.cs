using ecommerce_libro_ulp_backend.Models;

public interface IFavoritoRepository : IRepository<Favorito>
{
	abstract Task<List<Favorito>> ObtenerPorUsuarioId(int id);
	abstract Task<List<Favorito>> ObtenerPorUsuario(Usuario id);
	abstract Task<Favorito?> ObtenerPorId(int usuario_id, int libro_id);
}