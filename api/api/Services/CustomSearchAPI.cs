using Google.Apis.Customsearch.v1;
using Google.Apis.Services;

namespace api.Services
{
    public class CustomSearchAPI : ICustomSearchAPI
    {
        private const string apiKey = "AIzaSyAvfbmomN7vln1pih02C76TqTocNxNTa-4";
        private const string cx = "38bfb70cea59eed25";

        public string GetLyrics(string title)
        { 
            var customSearchService = new CustomsearchService(new BaseClientService.Initializer
            {
                ApiKey = apiKey
            });

            CseResource.ListRequest listRequest = customSearchService.Cse.List();
            listRequest.Cx = cx;
            listRequest.Q = title + "lyrics";

            var paging = listRequest.Execute().Items;

            if (paging != null && paging.Count > 0) return paging[0].FormattedUrl;
            return null;
        }
    }
}
