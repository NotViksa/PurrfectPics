using PurrfectPics.Data.Models;
using System.Linq.Expressions;

namespace PurrfectPics.Data.Interfaces
{
    public interface ICatImageRepository : IRepository<CatImage>
    {
        Task<IEnumerable<CatImage>> GetByTagAsync(string tagName);
        Task<IEnumerable<CatImage>> GetByUserAsync(string userId);
        Task<IEnumerable<CatImage>> GetMostPopularAsync(int count);
        Task<IEnumerable<CatImage>> GetRecentAsync(int count);
        Task<CatImage?> GetByIdWithDetailsAsync(int id);
        Task<int> CountAsync(Expression<Func<CatImage, bool>> predicate);
        Task<IEnumerable<CatImage>> SearchAsync(string searchTerm);
        IQueryable<CatImage> GetQueryable();

    }
}