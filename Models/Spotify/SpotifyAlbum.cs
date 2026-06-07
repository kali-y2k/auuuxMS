namespace auuuxMS.Models.Spotify;

public record SpotifyAlbum(
    string Id,
    string Name,
    string AlbumType,
    int TotalTracks,
    string ReleaseDate,
    SpotifyImage[] Images,
    SpotifyArtistSimple[] Artists,
    string SpotifyUrl
);

public record SpotifyArtistSimple(string Id, string Name);
