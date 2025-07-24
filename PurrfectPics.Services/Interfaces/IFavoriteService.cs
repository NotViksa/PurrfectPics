using PurrfectPics.Data.Models;

namespace PurrfectPics.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<bool> ToggleFavoriteAsync(string userId, int imageId);
        Task<bool> IsFavoritedAsync(string userId, int imageId);
        Task<IEnumerable<CatImage>> GetUserFavoritesAsync(string userId);
        Task<int> GetFavoriteCountByUserAsync(string userId);
        Task<IEnumerable<Favorite>> GetRecentFavoritesAsync(string userId, int count);
    }
}