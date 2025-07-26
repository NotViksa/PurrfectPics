using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data;
using PurrfectPics.Data.Models;
using PurrfectPics.Services.Interfaces;

public class CommentService : ICommentService
{
    private readonly ApplicationDbContext _context;

    public CommentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Comment> AddCommentAsync(string content, int catImageId, string userId)
    {
        var comment = new Comment
        {
            Content = content,
            CatImageId = catImageId,
            PostedById = userId,
            PostedDate = DateTime.UtcNow
        };

        await _context.Comments.AddAsync(comment);
        await _context.SaveChangesAsync();
        return comment;
    }

    public async Task<IEnumerable<Comment>> GetCommentsForImageAsync(int catImageId)
    {
        return await _context.Comments
            .Include(c => c.PostedBy)
            .Where(c => c.CatImageId == catImageId)
            .OrderByDescending(c => c.PostedDate)
            .ToListAsync();
    }

    public async Task<bool> DeleteCommentAsync(int commentId, string userId, bool isAdmin = false)
    {
        var comment = await _context.Comments
            .Include(c => c.PostedBy)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null) return false;

        if (comment.PostedById == userId || isAdmin)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }
    public async Task<int> GetCommentCountByUserAsync(string userId)
    {
        return await _context.Comments
            .CountAsync(c => c.PostedById == userId);
    }
    public async Task<bool> EditCommentAsync(int commentId, string userId, string newContent)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.Id == commentId && c.PostedById == userId);

        if (comment == null) return false;

        comment.Content = newContent;
        await _context.SaveChangesAsync();
        return true;
    }
}