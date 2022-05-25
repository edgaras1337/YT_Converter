using api.DTOs;
using System.Threading.Tasks;

namespace api.Services
{
    public interface IAudioService
    {
        Task<ConvertDTO> ConvertFile(string videoURL);
        Task<byte[]> GetFile(string fileName);
    }
}
