using Moq;
using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using PurrfectPics.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PurrfectPics.Tests.Services
{
    public class VoteServiceTests
    {
        private readonly Mock<IVoteRepository> _mockRepo;
        private readonly VoteService _service;

        public VoteServiceTests()
        {
            _mockRepo = new Mock<IVoteRepository>();
            _service = new VoteService(_mockRepo.Object);
        }

        [Fact]
        public async Task SubmitVoteAsync_AddsNewUpvote_WhenNoExistingVote()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetVoteAsync("user1", 1))
                .ReturnsAsync((Vote)null);
            _mockRepo.Setup(x => x.GetImageScoreAsync(1))
                .ReturnsAsync(1);

            // Act
            var result = await _service.SubmitVoteAsync("user1", 1, true);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(1, result.Score);
            _mockRepo.Verify(x => x.AddVoteAsync(It.Is<Vote>(v =>
                v.UserId == "user1" && v.CatImageId == 1 && v.IsUpvote)), Times.Once);
        }

        [Fact]
        public async Task SubmitVoteAsync_UpdatesVote_WhenChangingVote()
        {
            // Arrange
            var existingVote = new Vote { UserId = "user1", CatImageId = 1, IsUpvote = false };
            _mockRepo.Setup(x => x.GetVoteAsync("user1", 1))
                .ReturnsAsync(existingVote);
            _mockRepo.Setup(x => x.GetImageScoreAsync(1))
                .ReturnsAsync(1);

            // Act
            var result = await _service.SubmitVoteAsync("user1", 1, true);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(1, result.Score);
            _mockRepo.Verify(x => x.UpdateVoteAsync(existingVote), Times.Once);
            Assert.True(existingVote.IsUpvote);
        }

        [Fact]
        public async Task SubmitVoteAsync_RemovesVote_WhenTogglingSameVote()
        {
            // Arrange
            var existingVote = new Vote { UserId = "user1", CatImageId = 1, IsUpvote = true };
            _mockRepo.Setup(x => x.GetVoteAsync("user1", 1))
                .ReturnsAsync(existingVote);
            _mockRepo.Setup(x => x.GetImageScoreAsync(1))
                .ReturnsAsync(0);

            // Act
            var result = await _service.SubmitVoteAsync("user1", 1, true);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(0, result.Score);
            _mockRepo.Verify(x => x.RemoveVoteAsync(existingVote), Times.Once);
        }

        [Fact]
        public async Task GetImageScoreAsync_ReturnsCorrectScore()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetImageScoreAsync(1))
                .ReturnsAsync(5);

            // Act
            var result = await _service.GetImageScoreAsync(1);

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public async Task GetUserVoteAsync_ReturnsTrue_WhenUpvoted()
        {
            // Arrange
            var vote = new Vote { IsUpvote = true };
            _mockRepo.Setup(x => x.GetVoteAsync("user1", 1))
                .ReturnsAsync(vote);

            // Act
            var result = await _service.GetUserVoteAsync("user1", 1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetUserVoteAsync_ReturnsFalse_WhenDownvoted()
        {
            // Arrange
            var vote = new Vote { IsUpvote = false };
            _mockRepo.Setup(x => x.GetVoteAsync("user1", 1))
                .ReturnsAsync(vote);

            // Act
            var result = await _service.GetUserVoteAsync("user1", 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetUserVoteAsync_ReturnsNull_WhenNoVote()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetVoteAsync("user1", 1))
                .ReturnsAsync((Vote)null);

            // Act
            var result = await _service.GetUserVoteAsync("user1", 1);

            // Assert
            Assert.Null(result);
        }
    }
}