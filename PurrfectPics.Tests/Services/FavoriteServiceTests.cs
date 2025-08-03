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
    public class FavoriteServiceTests
    {
        private readonly Mock<IFavoriteRepository> _mockRepo;
        private readonly FavoriteService _service;

        public FavoriteServiceTests()
        {
            _mockRepo = new Mock<IFavoriteRepository>();
            _service = new FavoriteService(_mockRepo.Object);
        }

        [Fact]
        public async Task ToggleFavoriteAsync_AddsFavorite_WhenNotExists()
        {
            // Arrange
            var userId = "user1";
            var imageId = 1;
            _mockRepo.Setup(x => x.GetFavoriteAsync(userId, imageId))
                .ReturnsAsync((Favorite)null);

            // Act
            var result = await _service.ToggleFavoriteAsync(userId, imageId);

            // Assert
            Assert.True(result);
            _mockRepo.Verify(x => x.AddFavoriteAsync(It.Is<Favorite>(f =>
                f.UserId == userId && f.CatImageId == imageId)), Times.Once);
            _mockRepo.Verify(x => x.RemoveFavoriteAsync(It.IsAny<Favorite>()), Times.Never);
        }

        [Fact]
        public async Task ToggleFavoriteAsync_RemovesFavorite_WhenExists()
        {
            // Arrange
            var userId = "user1";
            var imageId = 1;
            var existingFavorite = new Favorite { UserId = userId, CatImageId = imageId };
            _mockRepo.Setup(x => x.GetFavoriteAsync(userId, imageId))
                .ReturnsAsync(existingFavorite);

            // Act
            var result = await _service.ToggleFavoriteAsync(userId, imageId);

            // Assert
            Assert.False(result);
            _mockRepo.Verify(x => x.RemoveFavoriteAsync(existingFavorite), Times.Once);
            _mockRepo.Verify(x => x.AddFavoriteAsync(It.IsAny<Favorite>()), Times.Never);
        }

        [Fact]
        public async Task IsFavoritedAsync_ReturnsTrue_WhenFavoriteExists()
        {
            // Arrange
            var userId = "user1";
            var imageId = 1;
            _mockRepo.Setup(x => x.FavoriteExistsAsync(userId, imageId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.IsFavoritedAsync(userId, imageId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsFavoritedAsync_ReturnsFalse_WhenFavoriteNotExists()
        {
            // Arrange
            _mockRepo.Setup(x => x.FavoriteExistsAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(false);

            // Act
            var result = await _service.IsFavoritedAsync("user1", 1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetUserFavoritesAsync_ReturnsImagesFromRepository()
        {
            // Arrange
            var expectedImages = new List<CatImage>
            {
                new CatImage { Id = 1, Title = "Fluffy" },
                new CatImage { Id = 2, Title = "Whiskers" }
            };
            _mockRepo.Setup(x => x.GetUserFavoriteImagesAsync("user1"))
                .ReturnsAsync(expectedImages);

            // Act
            var result = await _service.GetUserFavoritesAsync("user1");

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, i => i.Title == "Fluffy");
            Assert.Contains(result, i => i.Title == "Whiskers");
        }

        [Fact]
        public async Task GetFavoriteCountByUserAsync_ReturnsCorrectCount()
        {
            // Arrange
            var userId = "user1";
            _mockRepo.Setup(x => x.GetFavoriteCountByUserAsync(userId))
                .ReturnsAsync(5);

            // Act
            var result = await _service.GetFavoriteCountByUserAsync(userId);

            // Assert
            Assert.Equal(5, result);
        }

        [Fact]
        public async Task GetRecentFavoritesAsync_ReturnsOrderedFavorites()
        {
            // Arrange
            var userId = "user1";
            var now = DateTime.UtcNow;
            var favorites = new List<Favorite>
            {
                new Favorite { FavoritedDate = now.AddDays(-2), CatImage = new CatImage { Id = 1 } },
                new Favorite { FavoritedDate = now, CatImage = new CatImage { Id = 2 } },
                new Favorite { FavoritedDate = now.AddDays(-1), CatImage = new CatImage { Id = 3 } }
            };
            _mockRepo.Setup(x => x.GetRecentFavoritesAsync(userId, 3))
                .ReturnsAsync(favorites.OrderByDescending(f => f.FavoritedDate).Take(3).ToList());

            // Act
            var result = await _service.GetRecentFavoritesAsync(userId, 3);

            // Assert
            Assert.Equal(3, result.Count());
            Assert.Equal(2, result.First().CatImage.Id); // Most recent first
            Assert.Equal(3, result.Skip(1).First().CatImage.Id);
            Assert.Equal(1, result.Last().CatImage.Id);
        }

        [Fact]
        public async Task GetRecentFavoritesAsync_ReturnsLimitedCount()
        {
            // Arrange
            var userId = "user1";
            _mockRepo.Setup(x => x.GetRecentFavoritesAsync(userId, 2))
                .ReturnsAsync(new List<Favorite> { new Favorite(), new Favorite() });

            // Act
            var result = await _service.GetRecentFavoritesAsync(userId, 2);

            // Assert
            Assert.Equal(2, result.Count());
        }
    }
}