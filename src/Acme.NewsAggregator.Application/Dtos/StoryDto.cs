
using System.Text.Json.Serialization;

namespace Acme.NewsAggregator.Application.Dtos
{    
    //I'm using record because the dto must be immutable.
    public record StoryDto
    {
        [JsonPropertyName("id")]
        public int Id { get; init; }

        [JsonPropertyName("title")]
        public string Title { get; init; } = string.Empty;

        [JsonPropertyName("url")]
        public string Url { get; init; } = string.Empty;

        [JsonPropertyName("by")]
        public string By { get; init; } = string.Empty;

        [JsonPropertyName("time")]
        public long Time { get; init; }

        [JsonPropertyName("score")]
        public int Score { get; init; }

        [JsonPropertyName("descendants")]
        public int Descendants { get; init; } // This is the comment count

        [JsonPropertyName("type")]
        public string Type { get; init; } = string.Empty;
    }
}
