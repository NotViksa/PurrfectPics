using PurrfectPics.Data.Models;

namespace PurrfectPics.Services.Interfaces
{
    public interface ICommentService
    {
        Task<int> GetCommentCountByUserAsync(string userId);
    }
}