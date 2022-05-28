using api.DTOs;
using System.Threading.Tasks;

namespace api.Services
{
    public interface IConverterService
    {
        Task<ResponseDTO> ConvertFileAsync(string videoURL);
        Task<byte[]> GetFileAsync(string fileName, bool isAudioOnly);
    }
}
