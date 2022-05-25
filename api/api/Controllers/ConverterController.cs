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

        [HttpPost("/convert")]
        public async Task<IActionResult> Convert(UrlDto dto)
        {
            var audio = await _audioService.ConvertFile(dto.URL);
            var video = await _videoService.ConvertFile(dto.URL);

            if (audio is null || video is null) return BadRequest("Invalid URL.");

            //return Ok(response);
        }
    }
}
