using PurrfectPics.Data.Models;

namespace PurrfectPics.Services.Interfaces
{
    public interface ICommentService
    {
        Task<int> GetCommentCountByUserAsync(string userId);
        Task<Comment> AddCommentAsync(string content, int catImageId, string userId);
        Task<IEnumerable<Comment>> GetCommentsForImageAsync(int catImageId);
        Task<bool> DeleteCommentAsync(int commentId, string userId);
    }
}