using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurrfectPics.Data.Models.Identity;
using PurrfectPics.Services;
using PurrfectPics.Services.Interfaces;
using PurrfectPics.Web.Models;

namespace PurrfectPics.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICatImageService _catImageService;
        private readonly IFavoriteService _favoriteService;

        public UserController(
            UserManager<ApplicationUser> userManager,
            ICatImageService catImageService,
            IFavoriteService favoriteService)
        {
            _userManager = userManager;
            _catImageService = catImageService;
            _favoriteService = favoriteService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            var userImages = await _catImageService.GetImagesByUserAsync(user.Id);
            var favorites = await _favoriteService.GetUserFavoritesAsync(user.Id);

            return View(new UserDashboardViewModel
            {
                User = user,
                UploadedImages = userImages,
                FavoriteImages = favorites
            });
        }


        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(new EditProfileViewModel
            {
                DisplayName = user.DisplayName,
                ProfileBio = user.ProfileBio
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            user.DisplayName = model.DisplayName;
            user.ProfileBio = model.ProfileBio;

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile-images");
                var uniqueFileName = $"{user.Id}_{Guid.NewGuid()}{Path.GetExtension(model.ProfileImage.FileName)}";
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                Directory.CreateDirectory(uploadsPath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                user.ProfileImageUrl = $"/profile-images/{uniqueFileName}";
            }

            await _userManager.UpdateAsync(user);
            return RedirectToAction("Dashboard");
        }
    }
}