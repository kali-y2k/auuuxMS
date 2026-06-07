using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace auuuxMS.Infrastructure;

public class SpotifyTokenProvider(IHttpClientFactory httpClientFactory, IConfiguration config)
{
    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public async Task<string> GetAccessTokenAsync(CancellationToken ct = default)
    {
        if (_cachedToken is not null && DateTime.UtcNow < _tokenExpiry)
            return _cachedToken;

        var clientId = config["Spotify:ClientId"]!;
        var clientSecret = config["Spotify:ClientSecret"]!;

        var client = httpClientFactory.CreateClient();
        var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));

        var request = new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
        {
            Headers = { Authorization = new AuthenticationHeaderValue("Basic", credentials) },
            Content = new FormUrlEncodedContent([new("grant_type", "client_credentials")])
        };

        var response = await client.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);
        _cachedToken = json.GetProperty("access_token").GetString()!;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(json.GetProperty("expires_in").GetInt32() - 60);

        return _cachedToken;
    }
}
