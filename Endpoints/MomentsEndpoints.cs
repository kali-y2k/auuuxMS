using auuuxMS.Models.Domain;

namespace auuuxMS.Endpoints;

public static class MomentsEndpoints
{
    public static void MapMomentsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/moments").WithTags("Moments");

        group.MapPost("/", (CreateMomentRequest request) =>
        {
            if (string.IsNullOrWhiteSpace(request.TrackSpotifyId))
                return Results.BadRequest("TrackSpotifyId is required for a Moment.");

            // TODO: salvare su DB con expires_at = now + 27 min
            return Results.Created("/moments/placeholder-id", new { message = "Moment created (DB not wired yet)" });
        })
        .WithName("CreateMoment")
        .WithSummary("Post a live Moment (expires in 27 minutes)");

        group.MapGet("/live", () =>
        {
            // TODO: recuperare dal DB i moments con expires_at > now()
            return Results.Ok(new { items = Array.Empty<MomentResponse>() });
        })
        .WithName("GetLiveMoments")
        .WithSummary("Get all currently live Moments");
    }
}
