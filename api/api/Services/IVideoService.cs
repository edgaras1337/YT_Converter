using api.DTOs;
using System.Threading.Tasks;

namespace api.Services
{
    public interface IVideoService
    {
        Task<ConvertDTO> ConvertFile(string videoURL);
        Task<byte[]> GetFile(string fileName);
    }
}
