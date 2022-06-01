namespace api.DTOs
{
    /// <summary>
    /// This class stores more details about the response.
    /// It is used in ResponseDTO object.
    /// </summary>
    public class VideoDetails
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PublishDate { get; set; }
        public string ThumbnailURL { get; set; }
    }
}
