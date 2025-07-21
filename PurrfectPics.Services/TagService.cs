using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data;
using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using PurrfectPics.Services.Interfaces;

namespace PurrfectPics.Services
{
    public class TagService : ITagService
    {
        private readonly IRepository<Tag> _tagRepository;
        private readonly ApplicationDbContext _dbContext;

        public TagService(IRepository<Tag> tagRepository, ApplicationDbContext dbContext)
        {
            _tagRepository = tagRepository;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _tagRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Tag>> GetPopularTagsAsync(int count)
        {
            return await _dbContext.Tags
                .Include(t => t.CatImages)
                .OrderByDescending(t => t.CatImages.Count)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Tag?> GetTagByNameAsync(string name)
        {
            return await _dbContext.Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }

        public async Task<Tag> EnsureTagExistsAsync(string name)
        {
            var existingTag = await GetTagByNameAsync(name);
            if (existingTag != null)
            {
                return existingTag;
            }

            var newTag = new Tag { Name = name };
            await _tagRepository.AddAsync(newTag);
            return newTag;
        }

        public async Task<IEnumerable<Tag>> GetTagsForImageAsync(int imageId)
        {
            return await _dbContext.Tags
                .Where(t => t.CatImages.Any(ci => ci.Id == imageId))
                .ToListAsync();
        }

        public async Task<int> GetTagUsageCountAsync(int tagId)
        {
            return await _dbContext.Tags
                .Where(t => t.Id == tagId)
                .Select(t => t.CatImages.Count)
                .FirstOrDefaultAsync();
        }
    }
}