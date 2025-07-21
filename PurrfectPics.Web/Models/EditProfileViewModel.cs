using Microsoft.AspNetCore.Http;

namespace PurrfectPics.Web.Models
{
    public class EditProfileViewModel
    {
        public string DisplayName { get; set; }
        public string ProfileBio { get; set; }
        public IFormFile ProfileImage { get; set; }
    }
}