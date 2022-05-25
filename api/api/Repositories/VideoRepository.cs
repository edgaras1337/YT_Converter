using api.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace api.Repositories
{
    public class VideoRepository : TRepository<DownloadedVideo>
    {
        private readonly ApplicationDbContext _context;

        public VideoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(DownloadedVideo video)
        {
            await _context.DownloadedVideos.AddAsync(video);
            await _context.SaveChangesAsync();
        }

        public async Task<DownloadedVideo> GetByURL(string URL)
        {
            return await _context.DownloadedVideos
                .SingleOrDefaultAsync(v => v.URL == URL);
        }
    }
}
