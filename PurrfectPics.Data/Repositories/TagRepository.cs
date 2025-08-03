using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data;
using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PurrfectPics.Data.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Tag> AddAsync(Tag tag)
        {
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task DeleteAsync(Tag tag)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<Tag?> GetByIdAsync(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public async Task<Tag?> GetByNameAsync(string name)
        {
            return await _context.Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Tag>> GetPopularTagsAsync(int count)
        {
            return await _context.Tags
                .Include(t => t.CatImages)
                .OrderByDescending(t => t.CatImages.Count)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tag>> GetTagsForImageAsync(int imageId)
        {
            return await _context.Tags
                .Where(t => t.CatImages.Any(ci => ci.Id == imageId))
                .ToListAsync();
        }

        public async Task<int> GetUsageCountAsync(int tagId)
        {
            return await _context.Tags
                .Where(t => t.Id == tagId)
                .Select(t => t.CatImages.Count)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Tag tag)
        {
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
        }
    }
}