using ecommerce_libro_ulp_backend.Data;
using ecommerce_libro_ulp_backend.Models;
using Microsoft.EntityFrameworkCore;

public class FavoritoRepository : IFavoritoRepository
{
	private readonly AppDbContext _context;

	public FavoritoRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task<Favorito?> ObtenerPorId(int id)
	{
		return await _context.Favoritos.FindAsync(id);
	}

	public async Task<List<Favorito>> ObtenerTodos()
	{
		return await _context.Favoritos.ToListAsync();
	}

	public async Task Crear(Favorito obj)
	{
		await _context.Favoritos.AddAsync(obj);
		await _context.SaveChangesAsync();
	}

	public async Task Eliminar(Favorito obj)
	{
		_context.Favoritos.Remove(obj);
		await _context.SaveChangesAsync();
	}

	public async Task Modificar(Favorito obj)
	{
		_context.Favoritos.Update(obj);
		await _context.SaveChangesAsync();
	}

	public async Task<List<Favorito>> ObtenerPorUsuarioId(int id)
	{
		return await _context.Favoritos
			.Where(f => f.UsuarioId.Equals(id))
			.ToListAsync();
	}

	public async Task<List<Favorito>> ObtenerPorUsuario(Usuario usuario)
	{
		return await _context.Favoritos
			.Where(f => f.UsuarioId.Equals(usuario.Id))
			.ToListAsync();
	}
}
