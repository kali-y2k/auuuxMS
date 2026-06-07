namespace auuuxMS.Models.Domain;

public enum FeedItemType { Connect, Rate, Moment }

public record FeedItem(
    Guid Id,
    FeedItemType Type,
    Guid? UserId,
    string? Username,
    string? UserAvatarUrl,
    string TargetName,
    string? CoverUrl,
    decimal? Rating,
    string? Content,
    string? MoodTag,
    DateTime CreatedAt,
    bool IsLive          // true solo per Moments ancora attivi
);

public record FeedResponse(
    IReadOnlyList<FeedItem> Items,
    string? NextCursor
);
