using auuuxMS.Models.Domain;

namespace auuuxMS.Endpoints;

public static class ConnectsEndpoints
{
    public static void MapConnectsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/connects").WithTags("Connects");

        group.MapPost("/", (CreateConnectRequest request) =>
        {
            var targetCount =
                (request.TrackSpotifyId  is not null ? 1 : 0) +
                (request.AlbumSpotifyId  is not null ? 1 : 0) +
                (request.ArtistSpotifyId is not null ? 1 : 0);

            if (targetCount != 1)
                return Results.BadRequest("Specify exactly one of: TrackSpotifyId, AlbumSpotifyId, ArtistSpotifyId.");

            // TODO: salvare su DB
            return Results.Created("/connects/placeholder-id", new { message = "Connect created (DB not wired yet)" });
        })
        .WithName("CreateConnect")
        .WithSummary("Create a new Connect (track, album or artist)");

        group.MapGet("/user/{userId:guid}", (Guid userId, int page = 1, int pageSize = 20) =>
        {
            // TODO: recuperare connects dell'utente dal DB
            return Results.Ok(new { userId, page, pageSize, items = Array.Empty<ConnectResponse>() });
        })
        .WithName("GetUserConnects")
        .WithSummary("Get all connects for a user");
    }
}
