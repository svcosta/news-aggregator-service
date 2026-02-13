using Acme.NewsAggregator.Application.Dtos;

namespace Acme.NewsAggregator.Application.Interfaces
{
    public interface INewsAggregatorService
    {
        public Task<IEnumerable<StoryDto>> GetBestStoriesAsync(int n);
    }
}
