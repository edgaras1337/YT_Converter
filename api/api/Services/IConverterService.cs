using api.DTOs;
using System.Threading.Tasks;

namespace api.Services
{
    public interface IConverterService
    {
        Task<ResponseDTO> ConvertFile(string videoURL);
        Task<byte[]> GetFile(string fileName, bool isAudioOnly);
    }
}
