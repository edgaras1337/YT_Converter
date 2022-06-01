using Google.Apis.Customsearch.v1;
using Google.Apis.Services;

namespace api.Services
{
    /// <summary>
    /// This class stores methods, which are used to find the lyrics of the video from the web.
    /// </summary>
    public class CustomSearchAPI : ICustomSearchAPI
    {
        /// <summary>
        /// Key of the API.
        /// </summary>
        private const string _apiKey = "AIzaSyAvfbmomN7vln1pih02C76TqTocNxNTa-4";
        /// <summary>
        /// Search engines identifier. Google is used here.
        /// </summary>
        private const string _cx = "38bfb70cea59eed25";

        /// <summary>
        /// This method retrieved the first URL, from the web, found by searching
        /// by the video name and adding "lyrics" to it.
        /// </summary>
        /// <param name="title">Title of the video.</param>
        /// <returns>URL of the page, which has the lyrics of the video
        /// or null if no lyrics were found.</returns>
        public string GetLyrics(string title)
        { 
            var customSearchService = new CustomsearchService(new BaseClientService.Initializer
            {
                ApiKey = _apiKey
            });

            CseResource.ListRequest listRequest = customSearchService.Cse.List();
            listRequest.Cx = _cx;
            listRequest.Q = title + "lyrics";

            var paging = listRequest.Execute().Items;

            if (paging != null && paging.Count > 0) return paging[0].FormattedUrl;
            return null;
        }
    }
}
