using PurrfectPics.Data.Models;

namespace PurrfectPics.Data.Interfaces
{
    public interface ICatImageRepository : IRepository<CatImage>
    {
        Task<IEnumerable<CatImage>> GetByTagAsync(string tagName);
        Task<IEnumerable<CatImage>> GetByUserAsync(string userId);
        Task<IEnumerable<CatImage>> GetMostPopularAsync(int count);
        Task<IEnumerable<CatImage>> GetRecentAsync(int count);
    }
}