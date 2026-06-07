using auuuxMS.Models.Domain;

namespace auuuxMS.Endpoints;

public static class RatesEndpoints
{
    public static void MapRatesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/rates").WithTags("Rates");

        group.MapPost("/", (CreateRateRequest request) =>
        {
            if (request.Rating < 0 || request.Rating > 10)
                return Results.BadRequest("Rating must be between 0.0 and 10.0.");

            var targetCount =
                (request.TrackSpotifyId is not null ? 1 : 0) +
                (request.AlbumSpotifyId is not null ? 1 : 0);

            if (targetCount != 1)
                return Results.BadRequest("Specify exactly one of: TrackSpotifyId, AlbumSpotifyId.");

            // TODO: salvare su DB (upsert — l'utente può aggiornare il proprio voto)
            return Results.Created("/rates/placeholder-id", new { message = "Rate created (DB not wired yet)" });
        })
        .WithName("CreateRate")
        .WithSummary("Rate a track or album (0.0 – 10.0)");

        group.MapGet("/user/{userId:guid}", (Guid userId, int page = 1, int pageSize = 20) =>
        {
            // TODO: recuperare rates dell'utente dal DB
            return Results.Ok(new { userId, page, pageSize, items = Array.Empty<RateResponse>() });
        })
        .WithName("GetUserRates")
        .WithSummary("Get all rates for a user");
    }
}
