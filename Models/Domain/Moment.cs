namespace auuuxMS.Models.Domain;

public record CreateMomentRequest(
    Guid? UserId,
    string TrackSpotifyId,
    string? Content,
    int? MoodTagId
);

public record MomentResponse(
    Guid Id,
    Guid? UserId,
    string TrackSpotifyId,
    string TrackName,
    string? Content,
    string? MoodTag,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    bool IsLive
);
