using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurrfectPics.Services.Interfaces;
using System.Threading.Tasks;

namespace PurrfectPics.Web.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagService _tagService;
        private readonly ICatImageService _catImageService;

        public TagController(ITagService tagService, ICatImageService catImageService)
        {
            _tagService = tagService;
            _catImageService = catImageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var popularTags = await _tagService.GetPopularTagsAsync(20);
            return View(popularTags);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string tagName)
        {
            var images = await _catImageService.GetImagesByTagAsync(tagName);
            if (images == null || !images.Any())
            {
                return NotFound();
            }

            ViewBag.TagName = tagName;
            return View(images);
        }

        [HttpGet]
        public IActionResult Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}