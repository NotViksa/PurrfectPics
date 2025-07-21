using PurrfectPics.Data.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurrfectPics.Data.Models
{
    public class Vote
    {
        public int Id { get; set; }
        public bool IsUpvote { get; set; }
        public DateTime VotedDate { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public int CatImageId { get; set; }
        public string UserId { get; set; }

        // Navigation Properties
        public CatImage CatImage { get; set; }
        public ApplicationUser User { get; set; }
    }
}