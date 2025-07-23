using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data;
using PurrfectPics.Data.Models;
using PurrfectPics.Services.Interfaces;

namespace PurrfectPics.Services
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDbContext _context;

        public CommentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCommentCountByUserAsync(string userId)
        {
            return await _context.Comments
                .CountAsync(c => c.PostedById == userId);
        }
    }
}