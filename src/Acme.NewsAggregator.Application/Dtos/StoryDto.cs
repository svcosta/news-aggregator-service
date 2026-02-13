
namespace Acme.NewsAggregator.Application.Dtos
{    
    //I'm using record because the dto must be immutable.
    public record StoryDto
    {
        public string Title { get; init; } = string.Empty;
        public string Uri { get; init; } = string.Empty;
        public string PostedBy { get; init; } = string.Empty;
        public DateTime Time { get; init; }
        public int Score { get; init; }
        public int CommentCount { get; init; }
    }
}
