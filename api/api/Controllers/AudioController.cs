using api.DTOs;
using api.Models;
using api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/audio")]
    [ApiController]
    public class AudioController : ControllerBase
    {
        private readonly IAudioService _audioService;
        public AudioController(IAudioService audioService) => _audioService = audioService;

        [HttpPost("convert")]
        public async Task<IActionResult> ConvertAsync([FromBody] UrlDto dto)
        {
            var response = await _audioService.ConvertFile(dto.URL);

            if (response is null) return BadRequest("Invalid URL.");
            return Ok(response);
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadAsync([FromRoute] string fileName)
        {
            var bytes = await _audioService.GetFile(fileName);
            if (bytes is null) return NotFound("File not found.");
            return File(bytes, "audio/mpeg");
        }
    }
}
