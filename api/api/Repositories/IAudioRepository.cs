using api.Models;
using System.Threading.Tasks;

namespace api.Repositories
{
    public interface IAudioRepository
    {
        Task Add(DownloadedAudio audio);
        Task<DownloadedAudio> GetByName(string fileName);
        Task<DownloadedAudio> GetByURL(string URL);
    }
}
