using api.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace api.Repositories
{
    public class AudioRepository : IAudioRepository
    {
        private readonly ApplicationDbContext _context;
        public AudioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Add(DownloadedAudio audio)
        {
            await _context.DownloadedAudios.AddAsync(audio);
            await _context.SaveChangesAsync();
        }

        public async Task<DownloadedAudio> GetByName(string fileName)
        {
            return await _context.DownloadedAudios
                .SingleOrDefaultAsync(f => f.FileName == fileName);
        }

        public async Task<DownloadedAudio> GetByURL(string fileName)
        {
            return await _context.DownloadedAudios
                .SingleOrDefaultAsync(f => f.URL == fileName);
        }
    }
}
