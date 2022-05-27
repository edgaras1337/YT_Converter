using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class DownloadedAudio
    {
        [Key]
        public string FileName { get; set; }
        public string URL { get; set; }
        public string MediaType { get; set; }
    }
}
