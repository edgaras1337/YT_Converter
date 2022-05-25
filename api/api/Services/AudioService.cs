using api.DTOs;
using api.HelperClasses;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;


namespace api.Services
{
    public class AudioService : IAudioService
    {
        private readonly TRepository<DownloadedAudio> _audioRepository;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AudioService(
            TRepository<DownloadedAudio> audioRepository, 
            IHttpContextAccessor httpContextAccessor, 
            IWebHostEnvironment hostEnvironment)
        {
            _audioRepository = audioRepository;
            _httpContextAccessor = httpContextAccessor;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<ConvertDTO> ConvertFile(string videoURL)
        {
            try
            {
                var audioModel = await _audioRepository.GetByURL(videoURL);
                if (audioModel != null)
                    return new ConvertDTO { Title = audioModel.Title, FileSource = 
                        Helpers.CreateSourcePathAudio(audioModel.FileName, _httpContextAccessor.HttpContext) };

                var youtube = new YoutubeClient();
                var video = await youtube.Videos.GetAsync(videoURL);

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);
                var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                var fileName = $"{Guid.NewGuid()}{Helpers.MP3}";
                var audioPath = Path.Combine(_hostEnvironment.WebRootPath, "Audio", fileName);

                await youtube.Videos.Streams.DownloadAsync(streamInfo, audioPath);

                await _audioRepository.Add(new DownloadedAudio
                {
                    FileName = fileName,
                    URL = videoURL,
                    MediaType = Helpers.MP3,
                    Title = video.Title,
                });

                return new ConvertDTO { Title = video.Title, FileSource = 
                    Helpers.CreateSourcePathAudio(fileName, _httpContextAccessor.HttpContext) };
            }
            catch
            {
                return null;
            }
        }

        public async Task<byte[]> GetFile(string fileName)
        {
            var filePath = Path.Combine(_hostEnvironment.WebRootPath, "Audio", fileName);

            if (!File.Exists(filePath)) return null;

            return await File.ReadAllBytesAsync(filePath);

        }
    }
}
