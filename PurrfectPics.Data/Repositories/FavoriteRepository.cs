using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data;
using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PurrfectPics.Data.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoriteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Favorite> GetFavoriteAsync(string userId, int imageId)
        {
            return await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.CatImageId == imageId);
        }

        public async Task AddFavoriteAsync(Favorite favorite)
        {
            await _context.Favorites.AddAsync(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFavoriteAsync(Favorite favorite)
        {
            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> FavoriteExistsAsync(string userId, int imageId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.CatImageId == imageId);
        }

        public async Task<IEnumerable<CatImage>> GetUserFavoriteImagesAsync(string userId)
        {
            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.CatImage)
                .ThenInclude(ci => ci.Tags)
                .Select(f => f.CatImage)
                .ToListAsync();
        }

        public async Task<int> GetFavoriteCountByUserAsync(string userId)
        {
            return await _context.Favorites
                .CountAsync(f => f.UserId == userId);
        }

        public async Task<IEnumerable<Favorite>> GetRecentFavoritesAsync(string userId, int count)
        {
            return await _context.Favorites
                .Include(f => f.CatImage)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.FavoritedDate)
                .Take(count)
                .ToListAsync();
        }
    }
}