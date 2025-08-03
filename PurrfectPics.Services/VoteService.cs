using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using PurrfectPics.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace PurrfectPics.Services
{
    public class VoteService : IVoteService
    {
        private readonly IVoteRepository _voteRepository;

        public VoteService(IVoteRepository voteRepository)
        {
            _voteRepository = voteRepository;
        }

        public async Task<(bool Success, int Score)> SubmitVoteAsync(string userId, int imageId, bool isUpvote)
        {
            var existingVote = await _voteRepository.GetVoteAsync(userId, imageId);

            if (existingVote != null)
            {
                if (existingVote.IsUpvote == isUpvote)
                {
                    await _voteRepository.RemoveVoteAsync(existingVote);
                }
                else
                {
                    existingVote.IsUpvote = isUpvote;
                    existingVote.VotedDate = DateTime.UtcNow;
                    await _voteRepository.UpdateVoteAsync(existingVote);
                }
            }
            else
            {
                var vote = new Vote
                {
                    UserId = userId,
                    CatImageId = imageId,
                    IsUpvote = isUpvote,
                    VotedDate = DateTime.UtcNow
                };
                await _voteRepository.AddVoteAsync(vote);
            }

            var newScore = await _voteRepository.GetImageScoreAsync(imageId);
            return (true, newScore);
        }

        public async Task<int> GetImageScoreAsync(int imageId)
        {
            return await _voteRepository.GetImageScoreAsync(imageId);
        }

        public async Task<bool?> GetUserVoteAsync(string userId, int imageId)
        {
            var vote = await _voteRepository.GetVoteAsync(userId, imageId);
            return vote?.IsUpvote;
        }
    }
}