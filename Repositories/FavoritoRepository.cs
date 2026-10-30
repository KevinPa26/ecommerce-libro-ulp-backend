using ecommerce_libro_ulp_backend.Data;
using ecommerce_libro_ulp_backend.Models;
using Microsoft.EntityFrameworkCore;

public class FavoritoRepository : IFavoritoRepository
{
	private readonly DataContext _context;

	public FavoritoRepository(DataContext context)
	{
		_context = context;
	}

	public async Task<Favorito?> ObtenerPorId(int id)
	{
		return await _context.Favoritos.FindAsync(id);
	}

	public async Task<Favorito?> ObtenerPorId(int usuario_id, int libro_id)
	{
		return await _context.Favoritos
			.Where(f => f.UsuarioId.Equals(usuario_id) && f.LibroId.Equals(libro_id))
			.FirstOrDefaultAsync();
	}

	public async Task<List<Favorito>> ObtenerTodos()
	{
		return await _context.Favoritos
			.Include(f => f.Libro)
			.Include(f => f.Usuario)
			.ToListAsync();
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
			.Include(f => f.Libro)
			.Include(f => f.Usuario)
			.ToListAsync();
	}

	public async Task<List<Favorito>> ObtenerPorUsuario(Usuario usuario)
	{
		return await ObtenerPorUsuarioId(usuario.Id);
	}

}
