using Acme.NewsAggregator.Domain;

namespace Acme.NewsAggregator.Application.Interfaces
{
    public interface INewsAggregatorRepository
    {
        void Add(StoryEntity bookEntity);

        IEnumerable<StoryEntity> GetAll();

    }
}
