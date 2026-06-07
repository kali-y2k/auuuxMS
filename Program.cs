using auuuxMS.Endpoints;
using auuuxMS.Infrastructure;
using auuuxMS.Services;
using auuuxMS.Services.Interfaces;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHttpClient("Spotify", client =>
{
    client.BaseAddress = new Uri("https://api.spotify.com/v1/");
});
builder.Services.AddHttpClient();

builder.Services.AddSingleton<SpotifyTokenProvider>();
builder.Services.AddScoped<ISpotifyService, SpotifyService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Spotify
app.MapArtistsEndpoints();
app.MapAlbumsEndpoints();
app.MapSearchEndpoints();

// App
app.MapConnectsEndpoints();
app.MapRatesEndpoints();
app.MapMomentsEndpoints();
app.MapFeedEndpoints();
app.MapUsersEndpoints();

app.Run();
