using Moq;
using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using PurrfectPics.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PurrfectPics.Tests.Services
{
    public class TagServiceTests
    {
        private readonly Mock<ITagRepository> _mockRepo;
        private readonly TagService _service;

        public TagServiceTests()
        {
            _mockRepo = new Mock<ITagRepository>();
            _service = new TagService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllTagsAsync_ReturnsAllTagsFromRepository()
        {
            // Arrange
            var tags = new List<Tag>
            {
                new Tag { Id = 1, Name = "Tag1" },
                new Tag { Id = 2, Name = "Tag2" }
            };
            _mockRepo.Setup(x => x.GetAllAsync()).ReturnsAsync(tags);

            // Act
            var result = await _service.GetAllTagsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            _mockRepo.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPopularTagsAsync_ReturnsPopularTagsFromRepository()
        {
            // Arrange
            var tags = new List<Tag>
            {
                new Tag { Id = 1, Name = "Popular" },
                new Tag { Id = 2, Name = "LessPopular" }
            };
            _mockRepo.Setup(x => x.GetPopularTagsAsync(2)).ReturnsAsync(tags);

            // Act
            var result = await _service.GetPopularTagsAsync(2);

            // Assert
            Assert.Equal(2, result.Count());
            _mockRepo.Verify(x => x.GetPopularTagsAsync(2), Times.Once);
        }

        [Fact]
        public async Task GetTagByNameAsync_ReturnsTagFromRepository()
        {
            // Arrange
            var tag = new Tag { Id = 1, Name = "Test" };
            _mockRepo.Setup(x => x.GetByNameAsync("Test")).ReturnsAsync(tag);

            // Act
            var result = await _service.GetTagByNameAsync("Test");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            _mockRepo.Verify(x => x.GetByNameAsync("Test"), Times.Once);
        }

        [Fact]
        public async Task EnsureTagExistsAsync_ReturnsExistingTag_WhenExists()
        {
            // Arrange
            var existingTag = new Tag { Id = 1, Name = "Existing" };
            _mockRepo.Setup(x => x.GetByNameAsync("Existing")).ReturnsAsync(existingTag);

            // Act
            var result = await _service.EnsureTagExistsAsync("Existing");

            // Assert
            Assert.Equal(1, result.Id);
            _mockRepo.Verify(x => x.GetByNameAsync("Existing"), Times.Once);
            _mockRepo.Verify(x => x.AddAsync(It.IsAny<Tag>()), Times.Never);
        }

        [Fact]
        public async Task EnsureTagExistsAsync_CreatesNewTag_WhenNotExists()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetByNameAsync("New")).ReturnsAsync((Tag)null);
            _mockRepo.Setup(x => x.AddAsync(It.IsAny<Tag>()))
                .ReturnsAsync(new Tag { Id = 1, Name = "New" });

            // Act
            var result = await _service.EnsureTagExistsAsync("New");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            _mockRepo.Verify(x => x.GetByNameAsync("New"), Times.Once);
            _mockRepo.Verify(x => x.AddAsync(It.Is<Tag>(t => t.Name == "New")), Times.Once);
        }

        [Fact]
        public async Task GetTagsForImageAsync_ReturnsTagsFromRepository()
        {
            // Arrange
            var tags = new List<Tag>
            {
                new Tag { Id = 1, Name = "Tag1" }
            };
            _mockRepo.Setup(x => x.GetTagsForImageAsync(1)).ReturnsAsync(tags);

            // Act
            var result = await _service.GetTagsForImageAsync(1);

            // Assert
            Assert.Single(result);
            _mockRepo.Verify(x => x.GetTagsForImageAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetTagUsageCountAsync_ReturnsCountFromRepository()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetUsageCountAsync(1)).ReturnsAsync(5);

            // Act
            var result = await _service.GetTagUsageCountAsync(1);

            // Assert
            Assert.Equal(5, result);
            _mockRepo.Verify(x => x.GetUsageCountAsync(1), Times.Once);
        }
    }
}