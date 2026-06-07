namespace auuuxMS.Models.Spotify;

public record SpotifyArtist(
    string Id,
    string Name,
    IReadOnlyList<string> Genres,
    int Popularity,
    SpotifyImage[] Images,
    string SpotifyUrl
);

public record SpotifyImage(string Url, int? Width, int? Height);
