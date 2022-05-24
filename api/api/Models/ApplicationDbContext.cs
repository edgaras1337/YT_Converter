using Microsoft.EntityFrameworkCore;

namespace api.Models
{
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
