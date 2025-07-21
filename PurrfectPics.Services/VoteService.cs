using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data;
using PurrfectPics.Data.Models;
using PurrfectPics.Services.Interfaces;

namespace PurrfectPics.Services
{
    public class VoteService : IVoteService
    {
        private readonly ApplicationDbContext _context;

        public VoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, int Score)> SubmitVoteAsync(string userId, int imageId, bool isUpvote)
        {
            var existingVote = await _context.Votes
                .FirstOrDefaultAsync(v => v.UserId == userId && v.CatImageId == imageId);

            if (existingVote != null)
            {
                // User is changing their vote
                if (existingVote.IsUpvote == isUpvote)
                {
                    // User is clicking the same vote button - remove vote
                    _context.Votes.Remove(existingVote);
                }
                else
                {
                    // User is flipping their vote
                    existingVote.IsUpvote = isUpvote;
                    existingVote.VotedDate = DateTime.UtcNow;
                }
            }
            else
            {
                // New vote
                var vote = new Vote
                {
                    UserId = userId,
                    CatImageId = imageId,
                    IsUpvote = isUpvote
                };
                await _context.Votes.AddAsync(vote);
            }

            await _context.SaveChangesAsync();
            var newScore = await GetImageScoreAsync(imageId);
            return (true, newScore);
        }

        public async Task<int> GetImageScoreAsync(int imageId)
        {
            var votes = await _context.Votes
                .Where(v => v.CatImageId == imageId)
                .ToListAsync();

            return votes.Sum(v => v.IsUpvote ? 1 : -1);
        }

        public async Task<bool?> GetUserVoteAsync(string userId, int imageId)
        {
            var vote = await _context.Votes
                .FirstOrDefaultAsync(v => v.UserId == userId && v.CatImageId == imageId);

            return vote?.IsUpvote;
        }
    }
}