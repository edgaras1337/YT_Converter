using api.DTOs;
using api.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api")]
    [ApiController]
    public class ConverterController : ControllerBase
    {
        private readonly IConverterService _converterService;

        public ConverterController(IConverterService converterService) => _converterService = converterService;

        [HttpPost("convert")]
        //[ResponseCache(Duration = 180, VaryByQueryKeys = new string[] { "url" }, Location = ResponseCacheLocation.Any)]
        //public async Task<IActionResult> Convert(RequestDTO dto)
        public async Task<IActionResult> Convert([FromQuery] string url)
        {
            var response = await _converterService.ConvertFile(url);
            if (response is null) return BadRequest("Invalid URL.");
            return Ok(response);
        }

        [HttpGet("test")]
        [ResponseCache(Duration = 30)]
        public IActionResult Get(RequestDTO dto) 
        {
            return Ok(dto); 
        }

        [HttpGet("audio/download/{fileName}")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> DownloadAudioAsync([FromRoute] string fileName)
        {
            var bytes = await _converterService.GetFile(fileName, true);
            if (bytes is null) return NotFound("File not found.");
            return File(bytes, "audio/mpeg");
        }

        [HttpGet("video/download/{fileName}")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> DownloadVideoAsync([FromRoute] string fileName)
        {
            var bytes = await _converterService.GetFile(fileName, false);
            if (bytes is null) return NotFound("File not found.");
            return File(bytes, "video/mp4");
        }
    }
}
