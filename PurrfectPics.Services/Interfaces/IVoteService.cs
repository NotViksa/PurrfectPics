namespace PurrfectPics.Services.Interfaces
{
    public interface IVoteService
    {
        Task<(bool Success, int Score)> SubmitVoteAsync(string userId, int imageId, bool isUpvote);
        Task<int> GetImageScoreAsync(int imageId);
        Task<bool?> GetUserVoteAsync(string userId, int imageId);
    }
}