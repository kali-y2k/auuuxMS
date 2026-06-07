using auuuxMS.Models.Domain;

namespace auuuxMS.Endpoints;

public static class FeedEndpoints
{
    public static void MapFeedEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/feed").WithTags("Feed");

        group.MapGet("/", (Guid userId, string? cursor, int pageSize = 20) =>
        {
            // TODO: recuperare dal DB connects + rates + moments dell'utente, ordinati per created_at desc
            return Results.Ok(new FeedResponse([], null));
        })
        .WithName("GetMyFeed")
        .WithSummary("Get the personal activity feed (connects, rates, moments)");

        group.MapGet("/friends", (Guid userId, string? cursor, int pageSize = 20) =>
        {
            // TODO: recuperare dal DB le attività degli utenti seguiti
            return Results.Ok(new FeedResponse([], null));
        })
        .WithName("GetFriendsFeed")
        .WithSummary("Get the activity feed of followed users");
    }
}
