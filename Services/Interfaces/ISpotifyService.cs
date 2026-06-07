using auuuxMS.Models.Spotify;

namespace auuuxMS.Services.Interfaces;

public interface ISpotifyService
{
    Task<SpotifyArtist?> GetArtistAsync(string artistId, CancellationToken ct = default);
    Task<IReadOnlyList<SpotifyAlbum>> GetArtistAlbumsAsync(string artistId, CancellationToken ct = default);
    Task<SpotifyAlbum?> GetAlbumAsync(string albumId, CancellationToken ct = default);
    Task<SpotifySearchResult> SearchAsync(string query, CancellationToken ct = default);
}
