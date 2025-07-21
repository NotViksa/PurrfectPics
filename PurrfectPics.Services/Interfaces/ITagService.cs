using PurrfectPics.Data.Models;

namespace PurrfectPics.Services.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetAllTagsAsync();
        Task<IEnumerable<Tag>> GetPopularTagsAsync(int count);
        Task<Tag?> GetTagByNameAsync(string name);
        Task<Tag> EnsureTagExistsAsync(string name);
        Task<IEnumerable<Tag>> GetTagsForImageAsync(int imageId);
        Task<int> GetTagUsageCountAsync(int tagId);
    }
}