using Microsoft.EntityFrameworkCore;

namespace api.Models
{
    /// <summary>
    /// This class is used to retrieve data from the database.
    /// It stores a list of object, corresponding to a table in the database.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DownloadedAudio> DownloadedAudios => Set<DownloadedAudio>();
        public DbSet<DownloadedVideo> DownloadedVideos => Set<DownloadedVideo>();
    }
}
