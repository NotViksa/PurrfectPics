using PurrfectPics.Data.Models.Identity;

namespace PurrfectPics.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;

        public int CatImageId { get; set; }
        public CatImage CatImage { get; set; }

        public string PostedById { get; set; }
        public ApplicationUser PostedBy { get; set; }
    }
}