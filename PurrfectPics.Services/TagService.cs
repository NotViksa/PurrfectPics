using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using PurrfectPics.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurrfectPics.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await _tagRepository.GetAllAsync();
        }

        public async Task<IEnumerable<Tag>> GetPopularTagsAsync(int count)
        {
            return await _tagRepository.GetPopularTagsAsync(count);
        }

        public async Task<Tag?> GetTagByNameAsync(string name)
        {
            return await _tagRepository.GetByNameAsync(name);
        }

        public async Task<Tag> EnsureTagExistsAsync(string name)
        {
            var existingTag = await _tagRepository.GetByNameAsync(name);
            if (existingTag != null)
            {
                return existingTag;
            }

            var newTag = new Tag { Name = name };
            return await _tagRepository.AddAsync(newTag);
        }

        public async Task<IEnumerable<Tag>> GetTagsForImageAsync(int imageId)
        {
            return await _tagRepository.GetTagsForImageAsync(imageId);
        }

        public async Task<int> GetTagUsageCountAsync(int tagId)
        {
            return await _tagRepository.GetUsageCountAsync(tagId);
        }
    }
}