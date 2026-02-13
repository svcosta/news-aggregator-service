using Acme.NewsAggregator.Application.Dtos;
using Acme.NewsAggregator.Application.Interfaces;
using Acme.NewsAggregator.WebAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Acme.NewsAggregator.Service.Tests.Integration
{
    public class StoriesControllerTests
    {

        private readonly Mock<INewsAggregatorService> _serviceMock;
        private readonly StoriesController _controller;

        public StoriesControllerTests()
        {
            _serviceMock = new Mock<INewsAggregatorService>();
            _controller = new StoriesController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetBestStories_ReturnsBadRequest_WhenNIsZeroOrNegative()
        {
            // Act
            var result = await _controller.GetBestStories(0);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Count must be greater than zero.", badRequest.Value);

            _serviceMock.Verify(
                s => s.GetBestStoriesAsync(It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task GetBestStories_ReturnsOk_WithStories_WhenNIsValid()
        {
            // Arrange
            var stories = new[]
            {
                new StoryDto { Title = "Story 1", Score = 100 },
                new StoryDto { Title = "Story 2", Score = 50 }
            };

            _serviceMock
                .Setup(s => s.GetBestStoriesAsync(2))
                .ReturnsAsync(stories);

            // Act
            var result = await _controller.GetBestStories(2);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStories = Assert.IsAssignableFrom<IEnumerable<StoryDto>>(okResult.Value);

            Assert.Equal(2, returnedStories.Count());

            _serviceMock.Verify(
                s => s.GetBestStoriesAsync(2),
                Times.Once);
        }

        [Fact]
        public async Task GetBestStories_ReturnsOk_WithEmptyList_WhenServiceReturnsEmpty()
        {
            // Arrange
            _serviceMock
                .Setup(s => s.GetBestStoriesAsync(5))
                .ReturnsAsync(Enumerable.Empty<StoryDto>());

            // Act
            var result = await _controller.GetBestStories(5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStories = Assert.IsAssignableFrom<IEnumerable<StoryDto>>(okResult.Value);

            Assert.Empty(returnedStories);
        }
    }
}

