using api.DTOs;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace api.Services
{
    public class YoutubeAPI : IYoutubeAPI
    {
        private const string _key = "AIzaSyBw1JNYyFEWW4CZO4muQjZ0gYdWO-5_AwQ";

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
