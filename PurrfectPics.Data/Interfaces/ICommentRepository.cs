using PurrfectPics.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurrfectPics.Data.Interfaces
{
    public interface ICommentRepository
    {
        Task<Comment> GetByIdAsync(int commentId);
        Task<Comment> AddAsync(Comment comment);
        Task<bool> DeleteAsync(Comment comment);
        Task<IEnumerable<Comment>> GetCommentsForImageAsync(int catImageId);
        Task<int> GetCountByUserAsync(string userId);
        Task<bool> UpdateAsync(Comment comment);
    }
}