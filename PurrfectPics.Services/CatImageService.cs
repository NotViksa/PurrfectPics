using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using PurrfectPics.Services.Interfaces;

namespace PurrfectPics.Services
{
    public class CatImageService : ICatImageService
    {
        private readonly ICatImageRepository _catImageRepository;
        private readonly ITagService _tagService;

        public CatImageService(
            ICatImageRepository catImageRepository,
            ITagService tagService)
        {
            _catImageRepository = catImageRepository;
            _tagService = tagService;
        }

        public async Task<IEnumerable<CatImage>> GetAllImagesAsync()
        {
            return await _catImageRepository.GetAllAsync();
        }

        public async Task<CatImage?> GetImageByIdAsync(int id)
        {
            return await _catImageRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<IEnumerable<CatImage>> GetImagesByTagAsync(string tagName)
        {
            return await _catImageRepository.GetByTagAsync(tagName);
        }

        public async Task<IEnumerable<CatImage>> GetImagesByUserAsync(string userId)
        {
            return await _catImageRepository.GetByUserAsync(userId);
        }

        public async Task<IEnumerable<CatImage>> GetMostPopularImagesAsync(int count)
        {
            return await _catImageRepository.GetMostPopularAsync(count);
        }

        public async Task<IEnumerable<CatImage>> GetRecentImagesAsync(int count)
        {
            return await _catImageRepository.GetRecentAsync(count);
        }

        public async Task<CatImage> AddImageAsync(CatImage image, IEnumerable<string> tags)
        {
            var tagList = new List<Tag>();

            foreach (var tagName in tags)
            {
                var tag = await _tagService.EnsureTagExistsAsync(tagName);
                tagList.Add(tag);
            }

            image.Tags = tagList;
            await _catImageRepository.AddAsync(image);
            return image;
        }

        public async Task AddCommentAsync(Comment comment)
        {
            await _catImageRepository.UpdateAsync(comment.CatImage);
        }

        public async Task AddFavoriteAsync(Favorite favorite)
        {
            await _catImageRepository.UpdateAsync(favorite.CatImage);
        }

        public async Task AddVoteAsync(Vote vote)
        {
            await _catImageRepository.UpdateAsync(vote.CatImage);
        }

        public async Task<bool> DeleteImageAsync(int id)
        {
            try
            {
                var image = await _catImageRepository.GetByIdAsync(id);
                if (image == null) return false;

                await _catImageRepository.DeleteAsync(id);
                return true;
            }
            catch
            {
                // Log error here if needed
                return false;
            }
        }
    }
}