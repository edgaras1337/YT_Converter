using api.DTOs;
using System.Threading.Tasks;

namespace api.Services
{
    public interface IAudioService
    {
        //Task<byte[]> SaveMp3Audio(string videoURL);

        Task<ConvertDTO> ConvertAudio(string videoURL);
        Task<byte[]> GetAudioFile(string fileName);
    }
}
