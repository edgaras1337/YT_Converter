namespace api.DTOs
{
    /// <summary>
    /// This class is used to pass data to the client.
    /// It stores parameters, which a client can use.
    /// </summary>
    public class ResponseDTO
    {
        public string AudioURL { get; set; }
        public string VideoURL { get; set; }
        public string LyricsPageURL { get; set; }
        public VideoDetails Details { get; set; }
    }
}
