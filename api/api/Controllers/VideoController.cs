using api.DTOs;
using api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        public VideoController(IVideoService videoService) => _videoService = videoService;

        [HttpPost("convert")]
        public async Task<IActionResult> ConvertAsync([FromBody] UrlDto dto)
        {
            var response = await _videoService.ConvertFile(dto.URL);

            if (response is null) return BadRequest("Invalid URL.");
            return Ok(response);
        }

        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadAsync([FromRoute] string fileName)
        {
            var bytes = await _videoService.GetFile(fileName);
            if (bytes is null) return NotFound("File not found.");
            return File(bytes, "audio/mpeg");
        }
    }
}
