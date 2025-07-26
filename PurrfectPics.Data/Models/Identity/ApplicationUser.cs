using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PurrfectPics.Data.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Display name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Display name must be between 2-50 characters")]
        public string DisplayName { get; set; } = "New User";

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        public string? ProfileBio { get; set; }

        public string ProfileImageUrl { get; set; } = "/images/default-profile.png";

        // Navigation properties
        public ICollection<CatImage> UploadedImages { get; set; } = new List<CatImage>();
        public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
        public ICollection<Vote> Votes { get; set; } = new List<Vote>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    }
}