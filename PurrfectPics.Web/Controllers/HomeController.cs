using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PurrfectPics.Data.Models.Identity;
using PurrfectPics.Services.Interfaces;
using System.Security.Claims;

namespace PurrfectPics.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICatImageService _catImageService;
        private readonly ITagService _tagService;
        private readonly IFavoriteService _favoriteService;
        private readonly ICommentService _commentService;
        private readonly UserManager<ApplicationUser> _userManager;


        public HomeController(
            ICatImageService catImageService,
            ITagService tagService,
            IFavoriteService favoriteService,
            ICommentService commentService,
            UserManager<ApplicationUser> userManager)
        {
            _catImageService = catImageService;
            _tagService = tagService;
            _favoriteService = favoriteService;
            _commentService = commentService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("About", "Home");
            }

            var recentImages = await _catImageService.GetRecentImagesAsync(12);
            var popularTags = await _tagService.GetPopularTagsAsync(10);

            ViewBag.PopularTags = popularTags;
            return RedirectToAction("Dashboard");
        }

        public IActionResult About()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            ViewBag.UserDisplayName = user?.DisplayName ?? User.Identity?.Name;
            ViewBag.UploadCount = await _catImageService.GetImageCountByUserAsync(userId);
            ViewBag.FavoriteCount = await _favoriteService.GetFavoriteCountByUserAsync(userId);
            ViewBag.CommentCount = 0;

            // activity feed data
            ViewBag.RecentUploads = await _catImageService.GetRecentUserImagesAsync(userId, 3);
            ViewBag.RecentFavorites = await _favoriteService.GetRecentFavoritesAsync(userId, 3);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}