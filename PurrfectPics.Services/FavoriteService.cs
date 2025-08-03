using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using PurrfectPics.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurrfectPics.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;

        public FavoriteService(IFavoriteRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task<bool> ToggleFavoriteAsync(string userId, int imageId)
        {
            var existing = await _favoriteRepository.GetFavoriteAsync(userId, imageId);

            if (existing != null)
            {
                await _favoriteRepository.RemoveFavoriteAsync(existing);
                return false;
            }

            var favorite = new Favorite
            {
                UserId = userId,
                CatImageId = imageId,
                FavoritedDate = System.DateTime.UtcNow
            };

            await _favoriteRepository.AddFavoriteAsync(favorite);
            return true;
        }

        public async Task<bool> IsFavoritedAsync(string userId, int imageId)
        {
            return await _favoriteRepository.FavoriteExistsAsync(userId, imageId);
        }

        public async Task<IEnumerable<CatImage>> GetUserFavoritesAsync(string userId)
        {
            return await _favoriteRepository.GetUserFavoriteImagesAsync(userId);
        }

        public async Task<int> GetFavoriteCountByUserAsync(string userId)
        {
            return await _favoriteRepository.GetFavoriteCountByUserAsync(userId);
        }

        public async Task<IEnumerable<Favorite>> GetRecentFavoritesAsync(string userId, int count)
        {
            return await _favoriteRepository.GetRecentFavoritesAsync(userId, count);
        }
    }
}