using api.DTOs;
using api.Services;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Customsearch.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace api.Controllers
{
    /// <summary>
    /// This class derived from ControllerBase, holds methods which accept
    /// HTTP/HTTPS requests to the specified Controller Route '/api' with
    /// the controller's method's appended route.
    /// </summary>
    [Route("api")]
    [ApiController]
    public class ConverterController : ControllerBase
    {
        private readonly IConverterService _converterService;
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// Using Dependency Injection instances of classes which implement
        /// IConverterService and IMemoryCache are injected into the controller.
        /// </summary>
        /// <param name="converterService">ConverterService interface, which holds methods
        /// that are neccessary for converting a video to MP3 or MP4 formats.</param>
        /// <param name="memoryCache">MemoryCache instance, which is used to cache certain data.</param>
        public ConverterController(IConverterService converterService, IMemoryCache memoryCache)
        {
            _converterService = converterService;
            _memoryCache = memoryCache;
        } 

        /// <summary>
        /// This method accepts a request to convert a video to MP3 and MP4.
        /// It takes the result from _converterService and stores it into _memoryCache, 
        /// with the URL as a key, for repeated requests or from _memoryCache, if it exists there.
        /// </summary>
        /// <param name="url">URL of a video.</param>
        /// <returns>IActionResult, with either the ResponseDTO object, which holds
        /// response data, or with a message if any error occurs.</returns>
        [HttpPost("convert")]
        public async Task<IActionResult> ConvertAsync([FromQuery] string url)
        {
            if (!_memoryCache.TryGetValue(url, out ResponseDTO response))
            {
                response = await _converterService.ConvertFileAsync(url);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(60));

                if (response != null) _memoryCache.Set(url, response, cacheEntryOptions);
            }

            if (response is null) return BadRequest("Invalid URL!");
            return Ok(response);
        }

        /// <summary>
        /// This method accepts a request to download an MP3 file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>FileContentResult with the file ready for downloading with 
        /// caching parameters in the response header.</returns>
        [HttpGet("audio/download/{fileName}")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> DownloadAudioAsync([FromRoute] string fileName) => await GetFileAsync(fileName, true);

        /// <summary>
        /// This method accepts a request to download an MP3 file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>IActionResult with the file ready for downloading with 
        /// caching parameters in the response header.</returns>
        [HttpGet("video/download/{fileName}")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> DownloadVideoAsync([FromRoute] string fileName) =>  await GetFileAsync(fileName);

        /// <summary>
        /// This method is used as a helper method for DownloadAudioAsync and DownloadVideoAsync
        /// methods, to return the file bytes, using _converterService.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="isAudioOnly">Optional parameter to specify if MP3 or MP4 is requested.</param>
        /// <returns></returns>
        private async Task<IActionResult> GetFileAsync(string fileName, bool isAudioOnly = false)
        {
            var bytes = await _converterService.GetFileAsync(fileName, isAudioOnly);
            if (bytes is null) return NotFound("File not found.");
            return File(bytes, isAudioOnly ? "audio/mpeg" : "video/mp4");
        }
    }
}
