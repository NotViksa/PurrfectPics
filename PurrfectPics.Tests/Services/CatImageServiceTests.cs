using Moq;
using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using PurrfectPics.Services;
using PurrfectPics.Services.Interfaces;
using System.Linq.Expressions;
using Xunit;

namespace PurrfectPics.Tests.Services
{
    public class CatImageServiceTests
    {
        private readonly Mock<ICatImageRepository> _mockCatImageRepo;
        private readonly Mock<ITagService> _mockTagService;
        private readonly CatImageService _catImageService;

        public CatImageServiceTests()
        {
            _mockCatImageRepo = new Mock<ICatImageRepository>();
            _mockTagService = new Mock<ITagService>();
            _catImageService = new CatImageService(_mockCatImageRepo.Object, _mockTagService.Object);
        }

        // Tests

        [Fact]
        public async Task GetAllImagesAsync_ReturnsAllImages()
        {
            // Arrange
            var expectedImages = new List<CatImage>
    {
        new CatImage { Id = 1, Title = "Test Image 1" },
        new CatImage { Id = 2, Title = "Test Image 2" }
    };

            _mockCatImageRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expectedImages);

            // Act
            var result = await _catImageService.GetAllImagesAsync();

            // Assert
            Assert.Equal(2, result.Count());
            _mockCatImageRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetImageByIdAsync_ReturnsImage_WhenExists()
        {
            // Arrange
            var expectedImage = new CatImage { Id = 1, Title = "Test Image" };
            _mockCatImageRepo.Setup(repo => repo.GetByIdWithDetailsAsync(1))
                .ReturnsAsync(expectedImage);

            // Act
            var result = await _catImageService.GetImageByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedImage.Id, result.Id);
        }

        [Fact]
        public async Task GetImageByIdAsync_ReturnsNull_WhenNotExists()
        {
            // Arrange
            _mockCatImageRepo.Setup(repo => repo.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync((CatImage)null);

            // Act
            var result = await _catImageService.GetImageByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetImagesByTagAsync_ReturnsFilteredImages()
        {
            // Arrange
            var tagName = "cute";
            var expectedImages = new List<CatImage>
    {
        new CatImage { Id = 1, Title = "Cute Cat" }
    };

            _mockCatImageRepo.Setup(repo => repo.GetByTagAsync(tagName))
                .ReturnsAsync(expectedImages);

            // Act
            var result = await _catImageService.GetImagesByTagAsync(tagName);

            // Assert
            Assert.Single(result);
            Assert.Equal("Cute Cat", result.First().Title);
        }

        [Fact]
        public async Task AddImageAsync_CreatesNewImageWithTags()
        {
            // Arrange
            var newImage = new CatImage { Title = "New Image" };
            var tags = new List<string> { "cute", "fluffy" };

            _mockTagService.Setup(ts => ts.EnsureTagExistsAsync("cute"))
                .ReturnsAsync(new Tag { Id = 1, Name = "cute" });
            _mockTagService.Setup(ts => ts.EnsureTagExistsAsync("fluffy"))
                .ReturnsAsync(new Tag { Id = 2, Name = "fluffy" });

            _mockCatImageRepo.Setup(repo => repo.AddAsync(It.IsAny<CatImage>()))
                .Returns(Task.CompletedTask)
                .Callback<CatImage>(img => img.Id = 1);

            // Act
            var result = await _catImageService.AddImageAsync(newImage, tags);

            // Assert
            Assert.Equal(1, result.Id);
            Assert.Equal(2, result.Tags.Count);
            _mockTagService.Verify(ts => ts.EnsureTagExistsAsync(It.IsAny<string>()), Times.Exactly(2));
            _mockCatImageRepo.Verify(repo => repo.AddAsync(It.IsAny<CatImage>()), Times.Once);
        }

        [Fact]
        public async Task DeleteImageAsync_ReturnsTrue_WhenImageExists()
        {
            // Arrange
            var existingImage = new CatImage
            {
                Id = 1,
                Comments = new List<Comment>(),
                Votes = new List<Vote>(),
                Favorites = new List<Favorite>()
            };

            _mockCatImageRepo.Setup(repo => repo.GetByIdWithDetailsAsync(1))
                .ReturnsAsync(existingImage);

            _mockCatImageRepo.Setup(repo => repo.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _catImageService.DeleteImageAsync(1);

            // Assert
            Assert.True(result);
            _mockCatImageRepo.Verify(repo => repo.GetByIdWithDetailsAsync(1), Times.Once);
            _mockCatImageRepo.Verify(repo => repo.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteImageAsync_ClearsRelatedEntities_BeforeDeletion()
        {
            // Arrange
            var existingImage = new CatImage
            {
                Id = 1,
                Comments = new List<Comment> { new Comment() },
                Votes = new List<Vote> { new Vote() },
                Favorites = new List<Favorite> { new Favorite() }
            };

            _mockCatImageRepo.Setup(repo => repo.GetByIdWithDetailsAsync(1))
                .ReturnsAsync(existingImage);

            // Act
            var result = await _catImageService.DeleteImageAsync(1);

            // Assert
            Assert.True(result);
            Assert.Empty(existingImage.Comments);
            Assert.Empty(existingImage.Votes);
            Assert.Empty(existingImage.Favorites);
        }

        [Fact]
        public async Task DeleteImageAsync_ReturnsFalse_WhenImageNotFound()
        {
            // Arrange
            _mockCatImageRepo.Setup(repo => repo.GetByIdWithDetailsAsync(1))
                .ReturnsAsync((CatImage)null);

            // Act
            var result = await _catImageService.DeleteImageAsync(1);

            // Assert
            Assert.False(result);
            _mockCatImageRepo.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteImageAsync_ReturnsFalse_WhenRepositoryFails()
        {
            // Arrange
            var existingImage = new CatImage { Id = 1 };
            _mockCatImageRepo.Setup(repo => repo.GetByIdWithDetailsAsync(1))
                .ReturnsAsync(existingImage);
            _mockCatImageRepo.Setup(repo => repo.DeleteAsync(1))
                .ThrowsAsync(new Exception());

            // Act
            var result = await _catImageService.DeleteImageAsync(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteImageAsync_ReturnsFalse_WhenImageNotExists()
        {
            // Arrange
            _mockCatImageRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((CatImage)null);

            // Act
            var result = await _catImageService.DeleteImageAsync(999);

            // Assert
            Assert.False(result);
            _mockCatImageRepo.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteImageAsync_ReturnsFalse_OnException()
        {
            // Arrange
            var existingImage = new CatImage { Id = 1 };
            _mockCatImageRepo.Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(existingImage);
            _mockCatImageRepo.Setup(repo => repo.DeleteAsync(1))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _catImageService.DeleteImageAsync(1);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SearchImagesAsync_ReturnsRecentImages_WhenEmptySearchTerm()
        {
            // Arrange
            var recentImages = new List<CatImage>
    {
        new CatImage { Id = 1, Title = "Recent 1" },
        new CatImage { Id = 2, Title = "Recent 2" }
    };

            _mockCatImageRepo.Setup(repo => repo.GetRecentAsync(20))
                .ReturnsAsync(recentImages);

            // Act
            var result = await _catImageService.SearchImagesAsync("");

            // Assert
            Assert.Equal(2, result.Count());
            _mockCatImageRepo.Verify(repo => repo.GetRecentAsync(20), Times.Once);
            _mockCatImageRepo.Verify(repo => repo.SearchAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SearchImagesAsync_ReturnsFilteredResults_WhenSearchTermProvided()
        {
            // Arrange
            var searchTerm = "cute";
            var expectedResults = new List<CatImage>
    {
        new CatImage { Id = 1, Title = "Cute Cat" }
    };

            _mockCatImageRepo.Setup(repo => repo.SearchAsync(searchTerm))
                .ReturnsAsync(expectedResults);

            // Act
            var result = await _catImageService.SearchImagesAsync(searchTerm);

            // Assert
            Assert.Single(result);
            Assert.Equal("Cute Cat", result.First().Title);
            _mockCatImageRepo.Verify(repo => repo.GetRecentAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetRecentUserImagesAsync_ReturnsUserImagesOrderedByDate()
        {
            // Arrange
            var userId = "user123";
            var userImages = new List<CatImage>
    {
        new CatImage { Id = 1, UploadDate = DateTime.UtcNow.AddDays(-1) },
        new CatImage { Id = 2, UploadDate = DateTime.UtcNow }
    };

            _mockCatImageRepo.Setup(repo => repo.FindAsync(
                    It.IsAny<Expression<Func<CatImage, bool>>>(),
                    It.IsAny<Func<IQueryable<CatImage>, IOrderedQueryable<CatImage>>>(),
                    It.IsAny<int>()))
                .ReturnsAsync(userImages.OrderByDescending(i => i.UploadDate).Take(2).ToList());

            // Act
            var result = await _catImageService.GetRecentUserImagesAsync(userId, 2);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.True(result.First().UploadDate > result.Last().UploadDate);
        }
    }
}