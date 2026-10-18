using ecommerce_libro_ulp_backend.Data;
using ecommerce_libro_ulp_backend.Models;
using Microsoft.EntityFrameworkCore;

public class UsuarioRepository : IUsuarioRepository
{
	private readonly AppDbContext _context;

	public UsuarioRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task Crear(Usuario obj)
	{
		await _context.Usuarios.AddAsync(obj);
		await _context.SaveChangesAsync();
	}

	public async Task Eliminar(Usuario obj)
	{
		_context.Usuarios.Remove(obj);
		await _context.SaveChangesAsync();
	}

	public async Task Modificar(Usuario obj)
	{
		_context.Usuarios.Update(obj);
		await _context.SaveChangesAsync();
	}

	public async Task<Usuario?> ObtenerPorEmail(string email)
	{
		// Usamos FirstAsync, dado que email en la base de datos esta marcado como UNIQUE
		return await _context.Usuarios
			.Where(u => u.Activo.Equals(EstadoUsuario.ACTIVO) && u.Email.Contains(email))
			.FirstOrDefaultAsync();
	}

	public async Task<List<Usuario>> ObtenerPorEstado(EstadoUsuario estado)
	{
		return await _context.Usuarios
			.Where(u => u.Activo.Equals(estado))
			.ToListAsync();
	}

	public async Task<Usuario?> ObtenerPorId(int id)
	{
		return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
	}

	public async Task<List<Usuario>> ObtenerPorNombre(string nombre)
	{
		return await _context.Usuarios
			.Where(u => u.Nombre.Contains(nombre))
			.ToListAsync();
	}

	public async Task<List<Usuario>> ObtenerPorRol(RolUsuario rol)
	{
		return await _context.Usuarios
			.Where(u => u.Rol.Equals(rol))
			.ToListAsync();
	}

	public async Task<List<Usuario>> ObtenerTodos()
	{
		return await _context.Usuarios.ToListAsync();
	}
}