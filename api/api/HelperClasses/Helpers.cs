using Microsoft.AspNetCore.Http;

namespace api.HelperClasses
{
    public class Helpers
    {
        public const string MP3 = ".mp3";
        public const string MP4 = ".mp4";

        public static string CreateSourcePath(string fileName, HttpContext context) => 
            string.Format("{0}://{1}{2}/api/audio/download/{3}",
            context.Request.Scheme, context.Request.Host, context.Request.PathBase, fileName);
    }
}
