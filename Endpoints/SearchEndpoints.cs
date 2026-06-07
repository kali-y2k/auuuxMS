using auuuxMS.Services.Interfaces;

namespace auuuxMS.Endpoints;

public static class SearchEndpoints
{
    public static void MapSearchEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/search").WithTags("Search");

        group.MapGet("/", async (string q, ISpotifyService spotify, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(q))
                return Results.BadRequest("Query parameter 'q' is required.");

            var results = await spotify.SearchAsync(q, ct);
            return Results.Ok(results);
        })
        .WithName("Search")
        .WithSummary("Search artists and albums");
    }
}
