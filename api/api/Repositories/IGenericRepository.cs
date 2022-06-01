using System.Threading.Tasks;

namespace api.Repositories
{
    /// <summary>
    /// This interface is used, to instantiate repository classes for
    /// DownloadedAudio or DownloadedVideo models.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Adds an entity to the database.
        /// </summary>
        /// <param name="entity">Object to add.</param>
        /// <returns>Task, that should be awaited.</returns>
        Task Add(T entity);

        /// <summary>
        /// Gets an entity from the database by the URL.
        /// </summary>
        /// <param name="URL">URL of the DownloadedAudio or DownloadedVideo objects.</param>
        /// <returns>Task, that should be awaited, to retrieve the object.</returns>
        Task<T> GetByURL(string URL);
    }
}
