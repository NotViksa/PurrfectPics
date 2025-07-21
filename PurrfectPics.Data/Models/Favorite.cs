using PurrfectPics.Data.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PurrfectPics.Data.Models
{
    public class Favorite
    {
        public int Id { get; set; }
        public DateTime FavoritedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("CatImage")]
        public int CatImageId { get; set; }
        public CatImage CatImage { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}