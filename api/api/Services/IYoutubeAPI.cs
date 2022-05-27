using api.DTOs;
using System.Threading.Tasks;

namespace api.Services
{
    public interface IYoutubeAPI
    {
        Task<VideoDetails> GetVideoDetailsAsync(string videoID);
    }
}
