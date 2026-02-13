using Acme.NewsAggregator.Application.Dtos;
using Acme.NewsAggregator.Application.Interfaces;
using Acme.NewsAggregator.Domain;
using Acme.NewsAggregator.Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http.Json;

namespace Acme.NewsAggregator.Service.Tests.UnitTests
{
    public class NewsAggregatorServiceTests
    {
        private readonly Mock<INewsAggregatorRepository> _mockRepo;
        private readonly IMemoryCache _cache;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

        public NewsAggregatorServiceTests()
        {
            _mockRepo = new Mock<INewsAggregatorRepository>();
            // Using a real MemoryCache is more reliable than mocking Extension methods
            _cache = new MemoryCache(new MemoryCacheOptions());
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        }

        [Fact]
        public async Task GetBestStoriesAsync_ShouldReturnStories_WhenApiIsSuccessful()
        {
            // Arrange
            var storyIds = new[] { 1, 2 };
            var story1 = new StoryDto { Title = "Story 1", Score = 100, By = "Author 1" };
            var story2 = new StoryDto { Title = "Story 2", Score = 200, By = "Author 2" };

            // Mocking the sequence of HTTP calls
            SetupHttpMock("beststories.json", storyIds);
            SetupHttpMock("item/1.json", story1);
            SetupHttpMock("item/2.json", story2);

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/")
            };

            var service = new NewsAggregatorService(httpClient, _cache, _mockRepo.Object);

            // Act
            var result = (await service.GetBestStoriesAsync(2)).ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Story 2", result[0].Title); // Ordered by Score descending
            Assert.Equal(200, result[0].Score);

            // Verify repository was called
            _mockRepo.Verify(r => r.AddRange(It.IsAny<IEnumerable<StoryEntity>>()), Times.Once);
        }

        private void SetupHttpMock<T>(string urlPart, T responseContent)
        {
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().Contains(urlPart)),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(responseContent)
                });
        }
    }
}
