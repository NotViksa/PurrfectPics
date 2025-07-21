using Azure;
using PurrfectPics.Data.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace PurrfectPics.Data.Models
{
    public class CatImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string UploadedById { get; set; }
        public ApplicationUser UploadedBy { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
    }
}