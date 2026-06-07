using auuuxMS.Models.Domain;

namespace auuuxMS.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users").WithTags("Users");

        group.MapGet("/{id:guid}", (Guid id) =>
        {
            // TODO: recuperare profilo dal DB con contatori aggregati
            return Results.NotFound();
        })
        .WithName("GetUserProfile")
        .WithSummary("Get a user profile with stats");

        group.MapGet("/{id:guid}/diary", (Guid id, int year, int month) =>
        {
            if (month < 1 || month > 12)
                return Results.BadRequest("Month must be between 1 and 12.");

            // TODO: query DB — raggruppa connects+rates+moments per giorno nel mese
            return Results.Ok(new DiaryResponse(year, month, []));
        })
        .WithName("GetUserDiary")
        .WithSummary("Get calendar activity data for a user (year + month)");

        group.MapGet("/{id:guid}/wrapped", (Guid id, int year, int quarter) =>
        {
            if (quarter < 1 || quarter > 4)
                return Results.BadRequest("Quarter must be between 1 and 4.");

            // TODO: query DB — stats aggregate per trimestre
            return Results.Ok(new WrappedResponse(
                Period: $"Q{quarter} {year}",
                StartsAt: new DateTime(year, (quarter - 1) * 3 + 1, 1),
                EndsAt: new DateTime(year, quarter * 3, DateTime.DaysInMonth(year, quarter * 3)),
                ConnectsCount: 0,
                MomentsCount: 0,
                AverageRating: 0,
                ActiveDays: 0
            ));
        })
        .WithName("GetUserWrapped")
        .WithSummary("Get quarterly Wrapped stats for a user");

        group.MapGet("/{id:guid}/myvinyl", (Guid id) =>
        {
            // TODO: recuperare MyVinyl dal DB
            return Results.NotFound();
        })
        .WithName("GetMyVinyl")
        .WithSummary("Get a user's MyVinyl (12 identity tracks)");

        group.MapPut("/{id:guid}/myvinyl", (Guid id, UpdateMyVinylRequest request) =>
        {
            if (request.TrackSpotifyIds.Count > 12)
                return Results.BadRequest("MyVinyl can contain at most 12 tracks.");

            // TODO: aggiornare DB (verificare swaps_remaining prima di procedere)
            return Results.Ok(new { message = "MyVinyl updated (DB not wired yet)" });
        })
        .WithName("UpdateMyVinyl")
        .WithSummary("Update a user's MyVinyl");

        group.MapPut("/{id:guid}/tops", (Guid id, UpdateTopsRequest request) =>
        {
            if (request.ArtistSpotifyIds.Count > 3)
                return Results.BadRequest("Top artists: max 3.");
            if (request.AlbumSpotifyIds.Count > 3)
                return Results.BadRequest("Top albums: max 3.");

            // TODO: aggiornare DB
            return Results.Ok(new { message = "Tops updated (DB not wired yet)" });
        })
        .WithName("UpdateTops")
        .WithSummary("Update a user's Top 3 artists and albums");

        group.MapPost("/{id:guid}/follow", (Guid id, Guid followerId) =>
        {
            if (id == followerId)
                return Results.BadRequest("Cannot follow yourself.");

            // TODO: inserire su DB nella tabella follows
            return Results.Ok(new { message = "Followed (DB not wired yet)" });
        })
        .WithName("FollowUser")
        .WithSummary("Follow a user");

        group.MapDelete("/{id:guid}/follow", (Guid id, Guid followerId) =>
        {
            // TODO: cancellare da DB
            return Results.Ok(new { message = "Unfollowed (DB not wired yet)" });
        })
        .WithName("UnfollowUser")
        .WithSummary("Unfollow a user");
    }
}
