using api.DTOs;
using api.HelperClasses;
using api.Models;
using api.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace api.Services
{
    public class ConverterService : IConverterService
    {
        private readonly IGenericRepository<DownloadedAudio> _audioRepository;
        private readonly IGenericRepository<DownloadedVideo> _videoRepository;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomSearchAPI _customSearchAPI;
        private readonly IYoutubeAPI _youtubeAPI;

        public ConverterService(
            IGenericRepository<DownloadedAudio> audioRepository,
            IGenericRepository<DownloadedVideo> videoRepository,
            IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment hostEnvironment,
            ICustomSearchAPI customSearchAPI,
            IYoutubeAPI youtubeAPI)
        {
            _audioRepository = audioRepository;
            _videoRepository = videoRepository;
            _httpContextAccessor = httpContextAccessor;
            _hostEnvironment = hostEnvironment;
            _customSearchAPI = customSearchAPI;
            _youtubeAPI = youtubeAPI;
        }

        public async Task<ResponseDTO> ConvertFileAsync(string videoURL)
        {
            try
            {
                var youtube = new YoutubeClient();
                var video = await youtube.Videos.GetAsync(videoURL);

                var details = new VideoDetails();
                try
                {
                    // Get video details from YoutubeAPI.
                    details = await _youtubeAPI.GetVideoDetailsAsync(video.Id);
                }
                catch { }

                string lyricsPage = null;
                try
                {
                    // Get URL of website, which has lyrics of the video.
                    lyricsPage = _customSearchAPI.GetLyrics(details.Title);
                }
                catch { }


                // Get audio and video URL from the database.
                // Get data from there, if both are not null.
                var audioModel = await _audioRepository.GetByURL(videoURL);
                var videoModel = await _videoRepository.GetByURL(videoURL);
                if (audioModel != null && videoModel != null)
                {
                    return new ResponseDTO
                    {
                        AudioURL = CreateSourcePath(audioModel.FileName, "Audio"),
                        VideoURL = CreateSourcePath(videoModel.FileName, "Video"),
                        LyricsPageURL = lyricsPage,
                        Details = details,
                    };
                }

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(video.Id);

                // Download audio file, save it to .../wwwroot/Audio and save details of it in the database.
                var audioStreamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();
                var audioFileName = $"{Guid.NewGuid()}{MediaTypes.MP3}";
                var audioPath = Path.Combine(_hostEnvironment.WebRootPath, "Audio", audioFileName);
                await youtube.Videos.Streams.DownloadAsync(audioStreamInfo, audioPath);
                await _audioRepository.Add(new DownloadedAudio
                {
                    FileName = audioFileName,
                    URL = videoURL,
                    MediaType = MediaTypes.MP3,
                });


                // Download video file, save it to .../wwwroot/Video and save details of it in the database.
                var videoStreamInfo = streamManifest
                    .GetMuxedStreams()
                    .Where(s => s.Container == Container.Mp4)
                    .GetWithHighestVideoQuality();
                var videoFileName = $"{Guid.NewGuid()}{MediaTypes.MP4}";
                var videoPath = Path.Combine(_hostEnvironment.WebRootPath, "Video", videoFileName);
                await youtube.Videos.Streams.DownloadAsync(videoStreamInfo, videoPath);
                await _videoRepository.Add(new DownloadedVideo
                {
                    FileName = videoFileName,
                    URL = videoURL,
                    MediaType = MediaTypes.MP4,
                });

                return new ResponseDTO
                {
                    AudioURL = CreateSourcePath(audioFileName, "Audio"),
                    VideoURL = CreateSourcePath(videoFileName, "Video"),
                    LyricsPageURL = lyricsPage,
                    Details = details
                };
            }
            catch
            {
                return null;
            }
        }

        public async Task<byte[]> GetFileAsync(string fileName, bool isAudioOnly)
        {
            var folder = isAudioOnly ? "Audio" : "Video";
            var filePath = Path.Combine(_hostEnvironment.WebRootPath, folder, fileName);

            if (!File.Exists(filePath)) return null;

            return await File.ReadAllBytesAsync(filePath);

        }

        private string CreateSourcePath(string fileName, string folder) =>
            string.Format("{0}://{1}{2}/api/{3}/download/{4}",
            _httpContextAccessor.HttpContext.Request.Scheme,
            _httpContextAccessor.HttpContext.Request.Host,
            _httpContextAccessor.HttpContext.Request.PathBase, 
            folder.ToLower(), fileName);
    }
}
