using api.DTOs;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api")]
    [ApiController]
    public class ConverterController : ControllerBase
    {
        private readonly IAudioService _audioService;
        private readonly IVideoService _videoService;

        public ConverterController(IAudioService audioService, IVideoService videoService)
        {
            _audioService = audioService;
            _videoService = videoService;
        }
    }
}
