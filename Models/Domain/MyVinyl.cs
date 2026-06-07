namespace auuuxMS.Models.Domain;

public record MyVinylTrackItem(
    int Position,
    string TrackSpotifyId,
    string TrackTitle,
    string ArtistName
);

public record MyVinylResponse(
    Guid UserId,
    IReadOnlyList<MyVinylTrackItem> Tracks,
    int SwapsRemaining,
    DateTime UpdatedAt
);

public record UpdateMyVinylRequest(
    // Lista ordinata di spotify ID (max 12)
    IReadOnlyList<string> TrackSpotifyIds
);
