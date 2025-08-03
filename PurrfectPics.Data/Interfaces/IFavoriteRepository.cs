using PurrfectPics.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurrfectPics.Data.Interfaces
{
    public interface IFavoriteRepository
    {
        Task<Favorite> GetFavoriteAsync(string userId, int imageId);
        Task AddFavoriteAsync(Favorite favorite);
        Task RemoveFavoriteAsync(Favorite favorite);
        Task<bool> FavoriteExistsAsync(string userId, int imageId);
        Task<IEnumerable<CatImage>> GetUserFavoriteImagesAsync(string userId);
        Task<int> GetFavoriteCountByUserAsync(string userId);
        Task<IEnumerable<Favorite>> GetRecentFavoritesAsync(string userId, int count);
    }
}