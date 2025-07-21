using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data;
using PurrfectPics.Data.Models;
using PurrfectPics.Services.Interfaces;

namespace PurrfectPics.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly ApplicationDbContext _context;

        public FavoriteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ToggleFavoriteAsync(string userId, int imageId)
        {
            var existing = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.CatImageId == imageId);

            if (existing != null)
            {
                _context.Favorites.Remove(existing);
                await _context.SaveChangesAsync();
                return false;
            }

            var favorite = new Favorite
            {
                UserId = userId,
                CatImageId = imageId,
                FavoritedDate = DateTime.UtcNow
            };

            await _context.Favorites.AddAsync(favorite);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsFavoritedAsync(string userId, int imageId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserId == userId && f.CatImageId == imageId);
        }

        public async Task<IEnumerable<CatImage>> GetUserFavoritesAsync(string userId)
        {
            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.CatImage)
                .ThenInclude(ci => ci.Tags)
                .Select(f => f.CatImage)
                .ToListAsync();
        }
    }
}