using auuuxMS.Services.Interfaces;

namespace auuuxMS.Endpoints;

public static class ArtistsEndpoints
{
    public static void MapArtistsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/artists").WithTags("Artists");

        group.MapGet("/{id}", async (string id, ISpotifyService spotify, CancellationToken ct) =>
        {
            var artist = await spotify.GetArtistAsync(id, ct);
            return artist is null ? Results.NotFound() : Results.Ok(artist);
        })
        .WithName("GetArtist")
        .WithSummary("Get an artist by Spotify ID");

        group.MapGet("/{id}/albums", async (string id, ISpotifyService spotify, CancellationToken ct) =>
        {
            var albums = await spotify.GetArtistAlbumsAsync(id, ct);
            return Results.Ok(albums);
        })
        .WithName("GetArtistAlbums")
        .WithSummary("Get all albums for an artist");
    }
}
