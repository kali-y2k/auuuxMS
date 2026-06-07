namespace auuuxMS.Models.Spotify;

public record SpotifySearchResult(
    IReadOnlyList<SpotifyArtist> Artists,
    IReadOnlyList<SpotifyAlbum> Albums
);
