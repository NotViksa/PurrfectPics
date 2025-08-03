using Moq;
using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using PurrfectPics.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PurrfectPics.Tests.Services
{
    public class CommentServiceTests
    {
        private readonly Mock<ICommentRepository> _mockRepo;
        private readonly CommentService _service;

        public CommentServiceTests()
        {
            _mockRepo = new Mock<ICommentRepository>();
            _service = new CommentService(_mockRepo.Object);
        }

        [Fact]
        public async Task AddCommentAsync_CreatesNewComment()
        {
            // Arrange
            var newComment = new Comment
            {
                Content = "Test",
                CatImageId = 1,
                PostedById = "user1"
            };

            _mockRepo.Setup(x => x.AddAsync(It.IsAny<Comment>()))
                .ReturnsAsync(newComment);

            // Act
            var result = await _service.AddCommentAsync("Test", 1, "user1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Content);
            _mockRepo.Verify(x => x.AddAsync(It.Is<Comment>(c =>
                c.Content == "Test" && c.CatImageId == 1 && c.PostedById == "user1")), Times.Once);
        }

        [Fact]
        public async Task DeleteCommentAsync_ReturnsTrue_WhenUserIsOwner()
        {
            // Arrange
            var comment = new Comment { Id = 1, PostedById = "user1" };
            _mockRepo.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(comment);
            _mockRepo.Setup(x => x.DeleteAsync(comment))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteCommentAsync(1, "user1");

            // Assert
            Assert.True(result);
            _mockRepo.Verify(x => x.DeleteAsync(comment), Times.Once);
        }

        [Fact]
        public async Task DeleteCommentAsync_ReturnsTrue_WhenUserIsAdmin()
        {
            // Arrange
            var comment = new Comment { Id = 1, PostedById = "user2" };
            _mockRepo.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(comment);
            _mockRepo.Setup(x => x.DeleteAsync(comment))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteCommentAsync(1, "user1", true);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(x => x.DeleteAsync(comment), Times.Once);
        }

        [Fact]
        public async Task DeleteCommentAsync_ReturnsFalse_WhenUserNotAuthorized()
        {
            // Arrange
            var comment = new Comment { Id = 1, PostedById = "user2" };
            _mockRepo.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(comment);

            // Act
            var result = await _service.DeleteCommentAsync(1, "user1");

            // Assert
            Assert.False(result);
            _mockRepo.Verify(x => x.DeleteAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Fact]
        public async Task EditCommentAsync_ReturnsTrue_WhenUpdateSuccessful()
        {
            // Arrange
            var comment = new Comment { Id = 1, PostedById = "user1", Content = "Old" };
            _mockRepo.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(comment);
            _mockRepo.Setup(x => x.UpdateAsync(It.Is<Comment>(c => c.Content == "New")))
                .ReturnsAsync(true);

            // Act
            var result = await _service.EditCommentAsync(1, "user1", "New");

            // Assert
            Assert.True(result);
            _mockRepo.Verify(x => x.UpdateAsync(It.Is<Comment>(c => c.Content == "New")), Times.Once);
        }

        [Fact]
        public async Task EditCommentAsync_ReturnsFalse_WhenUserNotOwner()
        {
            // Arrange
            var comment = new Comment { Id = 1, PostedById = "user2" };
            _mockRepo.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(comment);

            // Act
            var result = await _service.EditCommentAsync(1, "user1", "New");

            // Assert
            Assert.False(result);
            _mockRepo.Verify(x => x.UpdateAsync(It.IsAny<Comment>()), Times.Never);
        }

        [Fact]
        public async Task GetCommentsForImageAsync_ReturnsCommentsFromRepository()
        {
            // Arrange
            var comments = new List<Comment>
            {
                new Comment { Id = 1, CatImageId = 1, Content = "Comment 1" },
                new Comment { Id = 2, CatImageId = 1, Content = "Comment 2" }
            };
            _mockRepo.Setup(x => x.GetCommentsForImageAsync(1))
                .ReturnsAsync(comments);

            // Act
            var result = await _service.GetCommentsForImageAsync(1);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Content == "Comment 1");
            Assert.Contains(result, c => c.Content == "Comment 2");
        }

        [Fact]
        public async Task GetCommentCountByUserAsync_ReturnsCountFromRepository()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetCountByUserAsync("user1"))
                .ReturnsAsync(5);

            // Act
            var result = await _service.GetCommentCountByUserAsync("user1");

            // Assert
            Assert.Equal(5, result);
        }
    }
}