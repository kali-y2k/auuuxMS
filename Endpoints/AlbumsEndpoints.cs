using auuuxMS.Services.Interfaces;

namespace auuuxMS.Endpoints;

public static class AlbumsEndpoints
{
    public static void MapAlbumsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/albums").WithTags("Albums");

        group.MapGet("/{id}", async (string id, ISpotifyService spotify, CancellationToken ct) =>
        {
            var album = await spotify.GetAlbumAsync(id, ct);
            return album is null ? Results.NotFound() : Results.Ok(album);
        })
        .WithName("GetAlbum")
        .WithSummary("Get an album by Spotify ID");
    }
}
