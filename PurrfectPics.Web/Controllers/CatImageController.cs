using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurrfectPics.Data.Models;
using PurrfectPics.Services;
using PurrfectPics.Services.Interfaces;
using System.Security.Claims;

namespace PurrfectPics.Web.Controllers
{
    public class CatImageController : Controller
    {
        private readonly ICatImageService _catImageService;
        private readonly ITagService _tagService;
        private readonly IFavoriteService _favoriteService;

        public CatImageController(
            ICatImageService catImageService,
            ITagService tagService)
        {
            _catImageService = catImageService;
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var image = await _catImageService.GetImageByIdAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return View(image);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(
            CatImage image,
            IFormFile imageFile,
            string tags)
        {
            if (!ModelState.IsValid)
            {
                return View(image);
            }

            // Handle image upload
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                Directory.CreateDirectory(uploadsPath);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                image.ImageUrl = $"/uploads/{uniqueFileName}";
            }

            image.UploadedById = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var tagList = tags?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            await _catImageService.AddImageAsync(image, tagList);
            return RedirectToAction("Details", new { id = image.Id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int imageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isNowFavorited = await _favoriteService.ToggleFavoriteAsync(userId, imageId);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true, isFavorited = isNowFavorited });
            }

            return RedirectToAction("Details", new { id = imageId });
        }
    }
}