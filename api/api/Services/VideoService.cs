using api.DTOs;
using api.HelperClasses;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace api.Services
{
    public class VideoService : IAudioService
    {
        private readonly TRepository<DownloadedVideo> _videoRepository;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VideoService(
            TRepository<DownloadedVideo> videoRepository,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment hostEnvironment)
        {
            _videoRepository = videoRepository;
            _httpContextAccessor = httpContextAccessor;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<ConvertDTO> ConvertFile(string videoURL)
        {
            try
            {
                var videoModel = await _videoRepository.GetByURL(videoURL);
                if (videoModel != null)
                    return new ConvertDTO
                    {
                        Title = videoModel.Title,
                        FileSource =
                        Helpers.CreateSourcePathAudio(videoModel.FileName, _httpContextAccessor.HttpContext)
                    };

                var youtube = new YoutubeClient();
                var video = await youtube.Videos.GetAsync(videoURL);

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                var streamInfo = streamManifest
                    .GetVideoOnlyStreams()
                    .Where(s => s.Container == Container.Mp4)
                    .GetWithHighestVideoQuality();

                var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

                var fileName = $"{Guid.NewGuid()}{Helpers.MP4}";
                var videoPath = Path.Combine(_hostEnvironment.WebRootPath, "Audio", fileName);
                await youtube.Videos.Streams.DownloadAsync(streamInfo, videoPath);

                await _videoRepository.Add(new DownloadedVideo
                {
                    FileName = fileName,
                    URL = videoURL,
                    MediaType = Helpers.MP4,
                    Title = video.Title,
                });

                return new ConvertDTO
                {
                    Title = video.Title,
                    FileSource = Helpers.CreateSourcePathVideo(fileName, _httpContextAccessor.HttpContext)
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<byte[]> GetFile(string fileName)
        {
            var filePath = Path.Combine(_hostEnvironment.WebRootPath, "Video", fileName);

            if (!File.Exists(filePath)) return null;

            return await File.ReadAllBytesAsync(filePath);
        }
    }
}
