using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data;
using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace PurrfectPics.Data.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly ApplicationDbContext _context;

        public VoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Vote> GetVoteAsync(string userId, int imageId)
        {
            return await _context.Votes
                .FirstOrDefaultAsync(v => v.UserId == userId && v.CatImageId == imageId);
        }

        public async Task AddVoteAsync(Vote vote)
        {
            await _context.Votes.AddAsync(vote);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateVoteAsync(Vote vote)
        {
            _context.Votes.Update(vote);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveVoteAsync(Vote vote)
        {
            _context.Votes.Remove(vote);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetImageScoreAsync(int imageId)
        {
            var votes = await _context.Votes
                .Where(v => v.CatImageId == imageId)
                .ToListAsync();

            return votes.Sum(v => v.IsUpvote ? 1 : -1);
        }
    }
}