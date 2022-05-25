using api.DTOs;
using api.Models;
using api.Repositories;
using System.Threading.Tasks;

namespace api.Services
{
    public class VideoService : IAudioService
    {
        private readonly TRepository<DownloadedVideo> _videoRepository;
        public VideoService(TRepository<DownloadedVideo> videoRepository)
        {
            _videoRepository = videoRepository;
        }

        public Task<ConvertDTO> ConvertFile(string videoURL)
        {
            throw new System.NotImplementedException();
        }

        public Task<byte[]> GetFile(string fileName)
        {
            throw new System.NotImplementedException();
        }
    }
}
