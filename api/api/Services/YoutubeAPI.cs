using api.DTOs;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace api.Services
{
    /// <summary>
    /// This class hold methods which are used to retrieve details about the video.
    /// </summary>
    public class YoutubeAPI : IYoutubeAPI
    {
        /// <summary>
        /// Key of the API.
        /// </summary>
        private const string _key = "AIzaSyBw1JNYyFEWW4CZO4muQjZ0gYdWO-5_AwQ";

        /// <summary>
        /// This method retrieves the video details, by using an external API.
        /// The ID of the video is passed, and the results which are needed are
        /// serialized from the JSON response of the API.
        /// </summary>
        /// <param name="videoID">ID of the video.</param>
        /// <returns>Task, which should be awaited, to get the VideoDetails object, or null if any
        /// errors occur.</returns>
        public async Task<VideoDetails> GetVideoDetailsAsync(string videoID)
        {
            var url = $"https://www.googleapis.com/youtube/v3/videos?part=snippet&id={videoID}&key={_key}";
            try
            {
                using var client = new HttpClient();
                using var res = await client.GetAsync(url);
                using var content = res.Content;
                var dataString = await content.ReadAsStringAsync();
                if (dataString != null)
                {
                    dynamic dataObj = JObject.Parse(dataString);

                    var x = dataObj["items"];
                    var y = dataObj.items[0].snippet;

                    return new VideoDetails
                    {
                        Title = dataObj.items[0].snippet.title,
                        Description = dataObj.items[0].snippet.description,
                        PublishDate = dataObj.items[0].snippet.publishedAt,
                        ThumbnailURL = dataObj.items[0].snippet.thumbnails.high.url,
                    };
                }
                else return null;
            }
            catch
            {
                return null;
            }
        }

    }
}
