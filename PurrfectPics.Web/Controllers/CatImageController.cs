using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurrfectPics.Data.Models;
using PurrfectPics.Services.Interfaces;
using System.Security.Claims;

namespace PurrfectPics.Web.Controllers
{
    public class CatImageController : Controller
    {
        private readonly ICatImageService _catImageService;
        private readonly ITagService _tagService;
        private readonly IFavoriteService _favoriteService;
        private readonly IVoteService _voteService;

        public CatImageController(
            ICatImageService catImageService,
            ITagService tagService,
            IFavoriteService favoriteService,
            IVoteService voteService)
        {
            _catImageService = catImageService;
            _tagService = tagService;
            _favoriteService = favoriteService;
            _voteService = voteService;
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

            image.UploadedById = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitVote(int imageId, bool isUpvote)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var (success, score) = await _voteService.SubmitVoteAsync(userId, imageId, isUpvote);

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success, score });
            }

            return RedirectToAction("Details", new { id = imageId });
        }

        [HttpGet]
        public async Task<IActionResult> GetVoteInfo(int imageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userVote = await _voteService.GetUserVoteAsync(userId, imageId);
            var score = await _voteService.GetImageScoreAsync(imageId);

            return Json(new { userVote, score });
        }
    }
}