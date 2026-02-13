using Acme.NewsAggregator.Application.Dtos;
using Acme.NewsAggregator.Application.Interfaces;
using Acme.NewsAggregator.Domain;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;

namespace Acme.NewsAggregator.Infrastructure.Services
{
    public sealed class NewsAggregatorService : INewsAggregatorService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private static readonly SemaphoreSlim _throttler = new(20); // Limit concurrent HN calls
        //If we want to persist the data on the database
        private readonly INewsAggregatorRepository _repository;

        public NewsAggregatorService(HttpClient httpClient, IMemoryCache cache, INewsAggregatorRepository repository)
        {
            _httpClient = httpClient;
            _cache = cache;
            _repository = repository;
        }     

        public async Task<IEnumerable<StoryDto>> GetBestStoriesAsync(int n)
        {
            var storyIds = await _httpClient.GetFromJsonAsync<int[]>("beststories.json");
            if (storyIds == null) return Enumerable.Empty<StoryDto>();

            // Fetch details in parallel, but throttled
            var tasks = storyIds.Take(n).Select(GetStoryWithCacheAsync);
            var stories = await Task.WhenAll(tasks);

            //persist on the database 

            SaveChanges(stories);

            return stories.OrderByDescending(s => s.Score);
        }


        private async Task<StoryDto> GetStoryWithCacheAsync(int id)
        {
            var story = await _cache.GetOrCreateAsync(
                $"story_{id}",
                async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                    await _throttler.WaitAsync();
                    try
                    {
                        var response =
                            await _httpClient.GetFromJsonAsync<StoryDto>($"item/{id}.json")
                            ?? throw new InvalidOperationException(
                                $"Hacker News API returned null for item {id}");

                        return new StoryDto
                        {
                            Title = response.Title ?? string.Empty,
                            Url = response.Url,
                            By = response.By ?? "unknown",
                            Time = response.Time,
                            Score = response.Score,
                            Descendants = response.Descendants
                        };
                    }
                    finally
                    {
                        _throttler.Release();
                    }
                });

            return story
                ?? throw new InvalidOperationException($"Cache returned null for story {id}");
        }

        /// <summary>
        /// Only if we want to persist the date on the database
        /// </summary>
        /// <param name="stories"></param>
        private void SaveChanges(StoryDto[] stories)
        {
            // Mapping dto to entity, real world I'd use auto mapper.
            var entities = stories.Select(dto =>
                 new StoryEntity(                                   
                     dto.Title,
                     dto.Url,
                     dto.By,
                     dto.Time,
                     dto.Score,
                     dto.Descendants
                 )
             );

            _repository.AddRange(entities);

        }


    }
}

