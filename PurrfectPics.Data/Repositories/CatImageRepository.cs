using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using System.Linq.Expressions;

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

        public async Task<CatImage?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.CatImages
                .Include(ci => ci.UploadedBy)
                .Include(ci => ci.Tags)
                .Include(ci => ci.Comments)
                .Include(ci => ci.Votes)
                .Include(ci => ci.Favorites)
                .FirstOrDefaultAsync(ci => ci.Id == id);
        }

        public async Task<int> CountAsync(Expression<Func<CatImage, bool>> predicate)
        {
            return await _context.CatImages.CountAsync(predicate);
        }
        public async Task<IEnumerable<CatImage>> SearchAsync(string searchTerm)
        {
            return await _context.CatImages
                .Include(ci => ci.Tags)
                .Include(ci => ci.UploadedBy)
                .Where(ci =>
                    ci.Title.Contains(searchTerm) ||
                    ci.Description.Contains(searchTerm) ||
                    ci.Tags.Any(t => t.Name.Contains(searchTerm))
                )
                .ToListAsync();
        }
        public IQueryable<CatImage> GetQueryable()
        {
            return _context.CatImages.AsQueryable();
        }
    }
}