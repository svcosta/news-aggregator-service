using Acme.NewsAggregator.Domain.Base;

namespace Acme.NewsAggregator.Domain;
public class StoryEntity : Entity
{
    // Using private setters to ensure the entity can only be modified 
    // through controlled logic or the constructor.
    public string Title { get; private set; }
    public string Uri { get; private set; }
    public string PostedBy { get; private set; }
    public long Time { get; private set; }
    public int Score { get; private set; }
    public int CommentCount { get; private set; }

    // Required by some ORMs or Serialization, but kept private
    private StoryEntity() { }

    public StoryEntity(
        string title,
        string uri,
        string postedBy,
        long time,
        int score,
        int commentCount)
    {
        // The entity should be able to validate itself. 
        this.Validate(title, uri, postedBy, score, commentCount);

        Title = title;
        Uri = uri;
        PostedBy = postedBy;
        Time = time;
        Score = score;
        CommentCount = commentCount;
    }

    /// <summary>
    /// Enforces business invariants for a News story.
    /// </summary>
    private void Validate(string title, string uri, string postedBy, int score, int commentCount)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(postedBy))
            throw new ArgumentException("Author (PostedBy) cannot be empty.", nameof(postedBy));

        if (score < 0)
            throw new ArgumentOutOfRangeException(nameof(score), "Score cannot be negative.");

        if (commentCount < 0)
            throw new ArgumentOutOfRangeException(nameof(commentCount), "Comment count cannot be negative.");

        // Simple URI validation;
        if (!string.IsNullOrEmpty(uri) && !System.Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            throw new ArgumentException("The provided URI is not a valid absolute URL.", nameof(uri));
    }
}