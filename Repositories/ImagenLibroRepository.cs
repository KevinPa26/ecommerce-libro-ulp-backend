using ecommerce_libro_ulp_backend.Data;
using ecommerce_libro_ulp_backend.Models;
using Microsoft.EntityFrameworkCore;

public class ImagenLibroRepository : IImagenLibroRepository
{
	private readonly DataContext _context;

	public ImagenLibroRepository(DataContext context)
	{
		_context = context;
	}

	public async Task<ImagenLibro?> ObtenerPorId(int id)
	{
		return await _context.ImagenesLibro.FindAsync(id);
	}

	public async Task<List<ImagenLibro>> ObtenerTodos()
	{
		return await _context.ImagenesLibro.ToListAsync();
	}

	public async Task Crear(ImagenLibro obj)
	{
		await _context.ImagenesLibro.AddAsync(obj);
		await _context.SaveChangesAsync();
	}

	public async Task Eliminar(ImagenLibro obj)
	{
		_context.ImagenesLibro.Remove(obj);
		await _context.SaveChangesAsync();
	}

	public async Task Modificar(ImagenLibro obj)
	{
		_context.ImagenesLibro.Update(obj);
		await _context.SaveChangesAsync();
	}

	public async Task<List<ImagenLibro>> ObtenerPorLibro(int libro_id)
	{
		return await _context.ImagenesLibro
			.Where(i => i.LibroId.Equals(libro_id))
			.ToListAsync();
	}

	public async Task<List<ImagenLibro>> ObtenerPorLibro(Libro libro)
	{
		return await _context.ImagenesLibro
			.Where(i => i.LibroId.Equals(libro.Id))
			.ToListAsync();
	}

	public async Task<ImagenLibro?> ObtenerImagenPrincipal(int libro_id)
	{
		return await _context.ImagenesLibro
			.Where(i => i.LibroId.Equals(libro_id) && i.EsPrincipal)
			.FirstOrDefaultAsync();
	}
	
	public async Task<ImagenLibro?> ObtenerImagenPrincipal(Libro libro)
	{
		return await _context.ImagenesLibro
			.Where(i => i.LibroId.Equals(libro.Id) && i.EsPrincipal)
			.FirstOrDefaultAsync();
	}
}
