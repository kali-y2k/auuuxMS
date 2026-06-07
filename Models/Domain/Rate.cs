namespace auuuxMS.Models.Domain;

public record CreateRateRequest(
    Guid? UserId,
    string? TrackSpotifyId,
    string? AlbumSpotifyId,
    decimal Rating,        // 0.0 – 10.0
    string? Content,
    int? MoodTagId
);

public record RateResponse(
    Guid Id,
    Guid? UserId,
    string TargetType,    // "track" | "album"
    string TargetSpotifyId,
    string TargetName,
    decimal Rating,
    string? Content,
    string? MoodTag,
    DateTime CreatedAt
);
