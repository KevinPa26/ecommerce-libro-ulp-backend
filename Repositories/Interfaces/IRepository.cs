public interface IRepository<T> where T : class
{
	abstract Task<T?> ObtenerPorId(int id);
	abstract Task<List<T>> ObtenerTodos();
	abstract Task Crear(T obj);
	abstract Task Eliminar(T obj);
	abstract Task Modificar(T obj);
}