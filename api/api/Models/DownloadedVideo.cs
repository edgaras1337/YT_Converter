using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    /// <summary>
    /// This class is used as a Model, to work with responses, or the database.
    /// </summary>
    public class DownloadedVideo
    {
        [Key]
        public string FileName { get; set; }
        public string URL { get; set; }
        public string MediaType { get; set; }
    }
}
