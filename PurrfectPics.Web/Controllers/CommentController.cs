using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurrfectPics.Services.Interfaces;
using System.Security.Claims;

namespace PurrfectPics.Web.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string content, int catImageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var comment = await _commentService.AddCommentAsync(content, catImageId, userId);
            return RedirectToAction("Details", "CatImage", new { id = catImageId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId, int catImageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _commentService.DeleteCommentAsync(commentId, userId);
            return RedirectToAction("Details", "CatImage", new { id = catImageId });
        }
    }
}
