namespace auuuxMS.Models.Domain;

public record CreateConnectRequest(
    Guid? UserId,
    string? TrackSpotifyId,
    string? AlbumSpotifyId,
    string? ArtistSpotifyId,
    string? Content,
    int? MoodTagId
);

public record ConnectResponse(
    Guid Id,
    Guid? UserId,
    string TargetType,   // "track" | "album" | "artist"
    string TargetSpotifyId,
    string TargetName,
    string? Content,
    string? MoodTag,
    DateTime CreatedAt
);
