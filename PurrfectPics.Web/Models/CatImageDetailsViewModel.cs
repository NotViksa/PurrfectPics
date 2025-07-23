using PurrfectPics.Data.Models;

namespace PurrfectPics.Web.Models
{
    public class CatImageDetailsViewModel
    {
        public CatImage Image { get; set; }
        public bool IsFavorited { get; set; }
        public bool? UserVote { get; set; } // true=upvote, false=downvote, null=no vote
        public int Score { get; set; }
    }
}