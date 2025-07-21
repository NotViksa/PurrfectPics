using PurrfectPics.Data.Models;
using PurrfectPics.Data.Models.Identity;

namespace PurrfectPics.Web.Models
{
    public class UserDashboardViewModel
    {
        public ApplicationUser User { get; set; }
        public IEnumerable<CatImage> UploadedImages { get; set; }

        public IEnumerable<CatImage> FavoriteImages { get; set; }
    }
}