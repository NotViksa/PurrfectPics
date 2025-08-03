using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using PurrfectPics.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurrfectPics.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task<Comment> AddCommentAsync(string content, int catImageId, string userId)
        {
            var comment = new Comment
            {
                Content = content,
                CatImageId = catImageId,
                PostedById = userId,
                PostedDate = System.DateTime.UtcNow
            };

            return await _commentRepository.AddAsync(comment);
        }

        public async Task<bool> DeleteCommentAsync(int commentId, string userId, bool isAdmin = false)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null) return false;

            if (comment.PostedById == userId || isAdmin)
            {
                return await _commentRepository.DeleteAsync(comment);
            }

            return false;
        }

        public async Task<bool> EditCommentAsync(int commentId, string userId, string newContent)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null || comment.PostedById != userId) return false;

            comment.Content = newContent;
            return await _commentRepository.UpdateAsync(comment);
        }

        public async Task<IEnumerable<Comment>> GetCommentsForImageAsync(int catImageId)
        {
            return await _commentRepository.GetCommentsForImageAsync(catImageId);
        }

        public async Task<int> GetCommentCountByUserAsync(string userId)
        {
            return await _commentRepository.GetCountByUserAsync(userId);
        }
    }
}