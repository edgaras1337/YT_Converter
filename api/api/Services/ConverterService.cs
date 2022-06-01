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
    /// <summary>
    /// This class holds the main business logic for converting and 
    /// retrieving audio/video files from YouTube.
    /// </summary>
    public class ConverterService : IConverterService
    {
        private readonly IGenericRepository<DownloadedAudio> _audioRepository;
        private readonly IGenericRepository<DownloadedVideo> _videoRepository;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomSearchAPI _customSearchAPI;
        private readonly IYoutubeAPI _youtubeAPI;

        /// <summary>
        /// All needed services are injected using DependencyInjection.
        /// </summary>
        /// <param name="audioRepository">AudioRepository instance.</param>
        /// <param name="videoRepository">VideoRepository instance.</param>
        /// <param name="httpContextAccessor">HttpContextAccessor instance, which is used for
        /// accessing the HttpContext.</param>
        /// <param name="hostEnvironment">WebHostEnvironment instance, which is used to
        /// access host's environment.</param>
        /// <param name="customSearchAPI">CustomSearchAPI instance.</param>
        /// <param name="youtubeAPI">YoutubeAPI instance.</param>
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

        /// <summary>
        /// This method convertes the file to both MP3 and MP4 and downloads both of them.
        /// First of all it checks, if the video information already exists in the database, if so
        /// it retrieves the video details from there, and the video from WebRoot (wwwroot) folder,
        /// where the video and audio files are stored in different folders.
        /// If the video does not exist in the database, it uses YoutubeAPI to retrieve details about the video
        /// and CustomSearchAPI to find the lyrics in the web, by the video name. Then it uses YoutubeExplode
        /// package, to do the conversion and store the details in the database and video and audio files
        /// in WebRoot.
        /// </summary>
        /// <param name="videoURL">URL of the video.</param>
        /// <returns>ResponseDTO on success, or null on failure.</returns>
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

        /// <summary>
        /// This method, retrieves the audio or video (based on the optional boolean parameter)
        /// from the WebRoot folder and converts it into a byte array.
        /// </summary>
        /// <param name="fileName">Name of the file to retrieve.</param>
        /// <param name="isAudioOnly">Optional parameter, to specify if MP3 or MP4 is requested.</param>
        /// <returns>Array of file bytes.</returns>
        public async Task<byte[]> GetFileAsync(string fileName, bool isAudioOnly)
        {
            var folder = isAudioOnly ? "Audio" : "Video";
            var filePath = Path.Combine(_hostEnvironment.WebRootPath, folder, fileName);

            if (!File.Exists(filePath)) return null;

            return await File.ReadAllBytesAsync(filePath);

        }

        /// <summary>
        /// This method is used as a helper method, to create a URL for the video/audio
        /// for client use.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="folder">Folder, in which the file is located: Audio or Video folders
        /// in the WebRoot folder.</param>
        /// <returns>URL of the file.</returns>
        private string CreateSourcePath(string fileName, string folder) =>
            string.Format("{0}://{1}{2}/api/{3}/download/{4}",
            _httpContextAccessor.HttpContext.Request.Scheme,
            _httpContextAccessor.HttpContext.Request.Host,
            _httpContextAccessor.HttpContext.Request.PathBase, 
            folder.ToLower(), fileName);
    }
}
