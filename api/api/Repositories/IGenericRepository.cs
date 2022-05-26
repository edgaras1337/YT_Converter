using System.Threading.Tasks;

namespace api.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task Add(T entity);
        Task<T> GetByURL(string URL);
    }
}
