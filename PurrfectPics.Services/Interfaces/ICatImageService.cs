using PurrfectPics.Data.Models;

namespace PurrfectPics.Services.Interfaces
{
    public interface ICatImageService
    {
        Task<IEnumerable<CatImage>> GetAllImagesAsync();
        Task<CatImage?> GetImageByIdAsync(int id);
        Task<IEnumerable<CatImage>> GetImagesByTagAsync(string tagName);
        Task<IEnumerable<CatImage>> GetImagesByUserAsync(string userId);
        Task<IEnumerable<CatImage>> GetMostPopularImagesAsync(int count);
        Task<IEnumerable<CatImage>> GetRecentImagesAsync(int count);
        Task<CatImage> AddImageAsync(CatImage image, IEnumerable<string> tags);
        Task AddCommentAsync(Comment comment);
        Task AddFavoriteAsync(Favorite favorite);
        Task AddVoteAsync(Vote vote);
        Task<bool> DeleteImageAsync(int id);
        Task<int> GetImageCountByUserAsync(string userId);
        Task<IEnumerable<CatImage>> SearchImagesAsync(string searchTerm);
        Task<IEnumerable<CatImage>> GetRecentUserImagesAsync(string userId, int count);
    }
}