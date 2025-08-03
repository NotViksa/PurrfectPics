using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data;
using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PurrfectPics.Data.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;

        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment> AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<bool> DeleteAsync(Comment comment)
        {
            _context.Comments.Remove(comment);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Comment> GetByIdAsync(int commentId)
        {
            return await _context.Comments
                .Include(c => c.PostedBy)
                .FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task<IEnumerable<Comment>> GetCommentsForImageAsync(int catImageId)
        {
            return await _context.Comments
                .Include(c => c.PostedBy)
                .Where(c => c.CatImageId == catImageId)
                .OrderByDescending(c => c.PostedDate)
                .ToListAsync();
        }

        public async Task<int> GetCountByUserAsync(string userId)
        {
            return await _context.Comments
                .CountAsync(c => c.PostedById == userId);
        }

        public async Task<bool> UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}