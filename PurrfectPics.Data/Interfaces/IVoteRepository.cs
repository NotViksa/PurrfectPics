using PurrfectPics.Data.Models;
using System.Threading.Tasks;

namespace PurrfectPics.Data.Interfaces
{
    public interface IVoteRepository
    {
        Task<Vote> GetVoteAsync(string userId, int imageId);
        Task AddVoteAsync(Vote vote);
        Task UpdateVoteAsync(Vote vote);
        Task RemoveVoteAsync(Vote vote);
        Task<int> GetImageScoreAsync(int imageId);
    }
}