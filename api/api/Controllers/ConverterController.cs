﻿using api.DTOs;
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
    [Route("api")]
    [ApiController]
    public class ConverterController : ControllerBase
    {
        private readonly IConverterService _converterService;
        private readonly IMemoryCache _memoryCache;

        public ConverterController(IConverterService converterService, IMemoryCache memoryCache)
        {
            _converterService = converterService;
            _memoryCache = memoryCache;
        } 

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

            response = await _converterService.ConvertFileAsync(url);
            if (response is null) return BadRequest("Invalid URL.");
            return Ok(response);
        }

        [HttpGet("audio/download/{fileName}")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> DownloadAudioAsync([FromRoute] string fileName) => await GetFileAsync(fileName, true);

        [HttpGet("video/download/{fileName}")]
        [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> DownloadVideoAsync([FromRoute] string fileName) =>  await GetFileAsync(fileName);

        private async Task<IActionResult> GetFileAsync(string fileName, bool isAudioOnly = false)
        {
            var bytes = await _converterService.GetFileAsync(fileName, isAudioOnly);
            if (bytes is null) return NotFound("File not found.");
            return File(bytes, isAudioOnly ? "audio/mpeg" : "video/mp4");
        }
    }
}
