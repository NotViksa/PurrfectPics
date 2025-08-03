using PurrfectPics.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurrfectPics.Data.Interfaces
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAllAsync();
        Task<Tag?> GetByIdAsync(int id);
        Task<Tag?> GetByNameAsync(string name);
        Task<Tag> AddAsync(Tag tag);
        Task UpdateAsync(Tag tag);
        Task DeleteAsync(Tag tag);
        Task<int> GetUsageCountAsync(int tagId);
        Task<IEnumerable<Tag>> GetTagsForImageAsync(int imageId);
        Task<IEnumerable<Tag>> GetPopularTagsAsync(int count);
    }
}