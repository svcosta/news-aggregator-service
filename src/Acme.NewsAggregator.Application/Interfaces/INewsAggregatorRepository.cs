using Acme.NewsAggregator.Domain;

namespace Acme.NewsAggregator.Application.Interfaces
{
    public interface INewsAggregatorRepository
    {
        void Add(StoryEntity storyEntity);
        void AddRange(IEnumerable<StoryEntity> storyEntities);
        IEnumerable<StoryEntity> GetAll();

    }
}
