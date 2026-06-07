namespace auuuxMS.Models.Domain;

public record UserProfileResponse(
    Guid Id,
    string Username,
    string? AvatarUrl,
    string? Bio,
    int ConnectsCount,
    int RatesCount,
    int MomentsCount,
    int FollowersCount,
    int FollowingCount
);

public record UpdateTopsRequest(
    IReadOnlyList<string> ArtistSpotifyIds,   // max 3, ordinati
    IReadOnlyList<string> AlbumSpotifyIds     // max 3, ordinati
);

public record DiaryDayEntry(
    DateOnly Date,
    int ActivityCount   // quante azioni in quel giorno (connect + rate + moment)
);

public record DiaryResponse(
    int Year,
    int Month,
    IReadOnlyList<DiaryDayEntry> Days
);

public record WrappedResponse(
    string Period,          // es. "Q1 2026"
    DateTime StartsAt,
    DateTime EndsAt,
    int ConnectsCount,
    int MomentsCount,
    decimal AverageRating,
    int ActiveDays
);
