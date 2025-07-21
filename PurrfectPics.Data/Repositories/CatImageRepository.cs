using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;

namespace PurrfectPics.Data.Repositories
{
    public class CatImageRepository : Repository<CatImage>, ICatImageRepository
    {
        public CatImageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CatImage>> GetByTagAsync(string tagName)
        {
            return await _context.CatImages
                .Include(ci => ci.Tags)
                .Include(ci => ci.UploadedBy)
                .Where(ci => ci.Tags.Any(t => t.Name == tagName))
                .ToListAsync();
        }

        public async Task<IEnumerable<CatImage>> GetByUserAsync(string userId)
        {
            return await _context.CatImages
                .Include(ci => ci.Tags)
                .Where(ci => ci.UploadedById == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CatImage>> GetMostPopularAsync(int count)
        {
            return await _context.CatImages
                .Include(ci => ci.Votes)
                .OrderByDescending(ci => ci.Votes.Count(v => v.IsUpvote) - ci.Votes.Count(v => !v.IsUpvote))
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<CatImage>> GetRecentAsync(int count)
        {
            return await _context.CatImages
                .OrderByDescending(ci => ci.UploadDate)
                .Take(count)
                .ToListAsync();
        }
    }
}