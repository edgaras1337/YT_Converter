using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class DownloadedVideo
    {
        [Key]
        public string FileName { get; set; }
        public string URL { get; set; }
        public string MediaType { get; set; }
        public string Title { get; set; }
    }
}
