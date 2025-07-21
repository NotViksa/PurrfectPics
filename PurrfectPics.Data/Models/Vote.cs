using PurrfectPics.Data.Models.Identity;

namespace PurrfectPics.Data.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public bool IsUpvote { get; set; } // true = upvote, false = downvote
        public DateTime VotedDate { get; set; } = DateTime.UtcNow;

        public int CatImageId { get; set; }
        public CatImage CatImage { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}