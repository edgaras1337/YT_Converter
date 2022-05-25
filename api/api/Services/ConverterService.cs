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
    public class ConverterService
    {
        private readonly TRepository<DownloadedAudio> _audioRepository;
        private readonly TRepository<DownloadedVideo> _videoRepository;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ConverterService(
            TRepository<DownloadedAudio> audioRepository,
            TRepository<DownloadedVideo> videoRepository,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment hostEnvironment)
        {
            _audioRepository = audioRepository;
            _videoRepository = videoRepository;
            _httpContextAccessor = httpContextAccessor;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<FileDTO> ConvertFile(string videoURL)
        {
            try
            {
                var audioModel = await _audioRepository.GetByURL(videoURL);
                var videoModel = await _videoRepository.GetByURL(videoURL);
                if (audioModel != null && videoModel != null)
                    return new FileDTO {
                        AudioURL = CreateSourcePath(audioModel.FileName, "Audio"),
                        VideoURL = CreateSourcePath(videoModel.FileName, "Video")
                    };

                var youtube = new YoutubeClient();
                var video = await youtube.Videos.GetAsync(videoURL);

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);

                // save audio
                var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                var audioFileName = $"{Guid.NewGuid()}{Helpers.MP3}";
                var audioPath = Path.Combine(_hostEnvironment.WebRootPath, "Audio", audioFileName);
                await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, audioPath);
                await _audioRepository.Add(new DownloadedAudio
                {
                    FileName = audioFileName,
                    URL = videoURL,
                    MediaType = Helpers.MP3,
                    Title = video.Title,
                });

                // save video
                var videoStreamInfo = streamManifest
                    .GetVideoOnlyStreams()
                    .Where(s => s.Container == Container.Mp4)
                    .GetWithHighestVideoQuality();
                //var videoStream = await youtube.Videos.Streams.GetAsync(videoStreamInfo);
                var videoFileName = $"{Guid.NewGuid()}{Helpers.MP4}";
                var videoPath = Path.Combine(_hostEnvironment.WebRootPath, "Video", videoFileName);
                await youtube.Videos.Streams.DownloadAsync(videoStreamInfo, videoPath);
                await _videoRepository.Add(new DownloadedVideo
                {
                    FileName = videoFileName,
                    URL = videoURL,
                    MediaType = Helpers.MP4,
                    Title = video.Title,
                });

                return new FileDTO
                {
                    AudioURL = CreateSourcePath(audioFileName, "Audio"),
                    VideoURL = CreateSourcePath(videoFileName, "Video")
                };
            }
            catch
            {
                return null;
            }
        }



        private string CreateSourcePath(string fileName, string folder) =>
            string.Format("{0}://{1}{2}/api/{3}/download/{4}",
            _httpContextAccessor.HttpContext.Request.Scheme,
            _httpContextAccessor.HttpContext.Request.Host,
            _httpContextAccessor.HttpContext.Request.PathBase, 
            folder, fileName);
    }
}
