using Microsoft.AspNetCore.Mvc;
using PurrfectPics.Services.Interfaces;

namespace PurrfectPics.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICatImageService _catImageService;
        private readonly ITagService _tagService;

        public HomeController(
            ICatImageService catImageService,
            ITagService tagService)
        {
            _catImageService = catImageService;
            _tagService = tagService;
        }

        public async Task<IActionResult> Index()
        {
            var recentImages = await _catImageService.GetRecentImagesAsync(12);
            var popularTags = await _tagService.GetPopularTagsAsync(10);

            ViewBag.PopularTags = popularTags;
            return View(recentImages);
        }

        public IActionResult About()
        {
            return View();
        }
    }
}