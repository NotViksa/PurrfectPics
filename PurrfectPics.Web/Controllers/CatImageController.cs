using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurrfectPics.Data.Models;
using PurrfectPics.Services.Interfaces;
using PurrfectPics.Web.Models;
using PurrfectPics.Web.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PurrfectPics.Web.Controllers
{
    public class CatImageController : Controller
    {
        private readonly ICatImageService _catImageService;
        private readonly ITagService _tagService;
        private readonly IFavoriteService _favoriteService;
        private readonly IVoteService _voteService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<CatImageController> _logger;
        private readonly ICommentService _commentService;

        public CatImageController(
            ICatImageService catImageService,
            ITagService tagService,
            IFavoriteService favoriteService,
            IVoteService voteService,
            IWebHostEnvironment environment,
            ILogger<CatImageController> logger,
            ICommentService commentService)
        {
            _catImageService = catImageService;
            _tagService = tagService;
            _favoriteService = favoriteService;
            _voteService = voteService;
            _environment = environment;
            _logger = logger;
            _commentService = commentService;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var image = await _catImageService.GetImageByIdAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            var comments = await _commentService.GetCommentsForImageAsync(id);

            var viewModel = new CatImageDetailsViewModel
            {
                CatImage = image,
                Comments = comments
            };

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.IsFavorited = await _favoriteService.IsFavoritedAsync(userId, id);
                ViewBag.UserVote = await _voteService.GetUserVoteAsync(userId, id);
            }
            ViewBag.Score = await _voteService.GetImageScoreAsync(id);

            return View(image);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Upload()
        {
            return View(new CatImageUploadViewModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(CatImageUploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate file
            if (model.ImageFile == null || model.ImageFile.Length == 0)
            {
                ModelState.AddModelError(nameof(model.ImageFile), "Please select an image file");
                return View(model);
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(model.ImageFile.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError(nameof(model.ImageFile), "Only JPG, PNG, and GIF files are allowed");
                return View(model);
            }

            // Validate file size (max 5MB)
            if (model.ImageFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(nameof(model.ImageFile), "File size cannot exceed 5MB");
                return View(model);
            }

            try
            {
                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsPath);

                // Generate unique filename
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                var image = new CatImage
                {
                    Title = model.Title,
                    Description = model.Description,
                    ImageUrl = $"/uploads/{uniqueFileName}",
                    UploadedById = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    UploadDate = DateTime.UtcNow
                };

                var tagList = model.Tags?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    ?? Array.Empty<string>();

                await _catImageService.AddImageAsync(image, tagList);

                return RedirectToAction("Details", new { id = image.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image");
                ModelState.AddModelError("", "An error occurred while uploading. Please try again.");
                return View(model);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFavorite(int imageId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isNowFavorited = await _favoriteService.ToggleFavoriteAsync(userId, imageId);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, isFavorited = isNowFavorited });
                }

                return RedirectToAction("Details", new { id = imageId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling favorite");
                return Json(new { success = false, error = "An error occurred" });
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitVote(int imageId, bool isUpvote)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var (success, score) = await _voteService.SubmitVoteAsync(userId, imageId, isUpvote);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success, score });
                }

                return RedirectToAction("Details", new { id = imageId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting vote");
                return Json(new { success = false, error = "An error occurred" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVoteInfo(int imageId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userVote = await _voteService.GetUserVoteAsync(userId, imageId);
                var score = await _voteService.GetImageScoreAsync(imageId);

                return Json(new { userVote, score });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vote info");
                return Json(new { error = "An error occurred" });
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var image = await _catImageService.GetImageByIdAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            // Check if current user is owner or admin
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (image.UploadedById != currentUserId && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            var success = await _catImageService.DeleteImageAsync(id);
            if (!success)
            {
                return NotFound();
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            const int pageSize = 8;
            var images = _catImageService.GetImagesQueryable();
            var paginatedImages = await PaginatedList<CatImage>.CreateAsync(images, pageNumber ?? 1, pageSize);
            return View(paginatedImages);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query, int? pageNumber)
        {
            const int pageSize = 8;
            var results = _catImageService.GetSearchQueryable(query);
            ViewBag.SearchQuery = query;
            var paginatedResults = await PaginatedList<CatImage>.CreateAsync(results, pageNumber ?? 1, pageSize);
            return View("Index", paginatedResults);
        }
    }
}