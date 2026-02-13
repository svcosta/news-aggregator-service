using Acme.NewsAggregator.Application.Interfaces;
using Acme.NewsAggregator.Domain;
using System.Collections.Concurrent;

namespace Acme.NewsAggregator.Infrastructure.Persistence
{
    //TODO: Replace to Entity Framework Context 
    public sealed class NewsAggregatorRepository : INewsAggregatorRepository
    {
        // Real world I would have an Entity Framework Context, as I don't have time now, I will use a dictorires as memory datebase.
        private readonly ConcurrentDictionary<Guid, StoryEntity> _stories = new();

        public void Add(StoryEntity storyEntity)
        {
            _stories[storyEntity.Id] = storyEntity;
        }

        public void AddRange(IEnumerable<StoryEntity> storyEntities)
        {
            if (storyEntities == null) return;

            foreach (var story in storyEntities)
            {
                _stories[story.Id] = story;
            }
        }

        public IEnumerable<StoryEntity> GetAll()
        {
            return _stories.Values;
        }
    }
}
