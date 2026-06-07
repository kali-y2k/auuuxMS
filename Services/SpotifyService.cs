using System.Net.Http.Headers;
using System.Text.Json;
using auuuxMS.Infrastructure;
using auuuxMS.Models.Spotify;
using auuuxMS.Services.Interfaces;

namespace auuuxMS.Services;

public class SpotifyService(IHttpClientFactory httpClientFactory, SpotifyTokenProvider tokenProvider) : ISpotifyService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private async Task<HttpClient> CreateAuthorizedClientAsync(CancellationToken ct)
    {
        var token = await tokenProvider.GetAccessTokenAsync(ct);
        var client = httpClientFactory.CreateClient("Spotify");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    public async Task<SpotifyArtist?> GetArtistAsync(string artistId, CancellationToken ct = default)
    {
        var client = await CreateAuthorizedClientAsync(ct);
        var json = await client.GetFromJsonAsync<JsonElement>($"artists/{artistId}", ct);
        return MapArtist(json);
    }

    public async Task<IReadOnlyList<SpotifyAlbum>> GetArtistAlbumsAsync(string artistId, CancellationToken ct = default)
    {
        var client = await CreateAuthorizedClientAsync(ct);
        var response = await client.GetAsync($"artists/{artistId}/albums?include_groups=album,single&market=IT", ct);
        var body = await response.Content.ReadAsStringAsync(ct);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Spotify {response.StatusCode}: {body}");
        var json = JsonSerializer.Deserialize<JsonElement>(body);
        return json.GetProperty("items").EnumerateArray().Select(MapAlbum).ToList();
    }

    public async Task<SpotifyAlbum?> GetAlbumAsync(string albumId, CancellationToken ct = default)
    {
        var client = await CreateAuthorizedClientAsync(ct);
        var json = await client.GetFromJsonAsync<JsonElement>($"albums/{albumId}", ct);
        return MapAlbum(json);
    }

    public async Task<SpotifySearchResult> SearchAsync(string query, CancellationToken ct = default)
    {
        var client = await CreateAuthorizedClientAsync(ct);
        var encoded = Uri.EscapeDataString(query);
        var json = await client.GetFromJsonAsync<JsonElement>($"search?q={encoded}&type=artist,album&limit=10", ct);

        var artists = json.GetProperty("artists").GetProperty("items").EnumerateArray().Select(MapArtist).ToList();
        var albums = json.GetProperty("albums").GetProperty("items").EnumerateArray().Select(MapAlbum).ToList();

        return new SpotifySearchResult(artists, albums);
    }

    private static SpotifyArtist MapArtist(JsonElement j) => new(
        Id: j.GetProperty("id").GetString()!,
        Name: j.GetProperty("name").GetString()!,
        Genres: j.TryGetProperty("genres", out var g) ? g.EnumerateArray().Select(x => x.GetString()!).ToList() : [],
        Popularity: j.TryGetProperty("popularity", out var p) ? p.GetInt32() : 0,
        Images: MapImages(j),
        SpotifyUrl: j.GetProperty("external_urls").GetProperty("spotify").GetString()!
    );

    private static SpotifyAlbum MapAlbum(JsonElement j) => new(
        Id: j.GetProperty("id").GetString()!,
        Name: j.GetProperty("name").GetString()!,
        AlbumType: j.GetProperty("album_type").GetString()!,
        TotalTracks: j.TryGetProperty("total_tracks", out var t) ? t.GetInt32() : 0,
        ReleaseDate: j.GetProperty("release_date").GetString()!,
        Images: MapImages(j),
        Artists: j.GetProperty("artists").EnumerateArray()
            .Select(a => new SpotifyArtistSimple(a.GetProperty("id").GetString()!, a.GetProperty("name").GetString()!))
            .ToArray(),
        SpotifyUrl: j.GetProperty("external_urls").GetProperty("spotify").GetString()!
    );

    private static SpotifyImage[] MapImages(JsonElement j) =>
        j.TryGetProperty("images", out var imgs)
            ? imgs.EnumerateArray().Select(i => new SpotifyImage(
                i.GetProperty("url").GetString()!,
                i.TryGetProperty("width", out var w) && w.ValueKind != JsonValueKind.Null ? w.GetInt32() : null,
                i.TryGetProperty("height", out var h) && h.ValueKind != JsonValueKind.Null ? h.GetInt32() : null
            )).ToArray()
            : [];
}
