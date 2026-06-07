using auuuxMS.Data;
using auuuxMS.Data.Entities;
using auuuxMS.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace auuuxMS.Endpoints;

public static class RatesEndpoints
{
    public static void MapRatesEndpoints(this WebApplication app)
    {
        // Raggruppa tutti gli endpoint sotto il prefisso /rates
        var group = app.MapGroup("/rates").WithTags("Rates");

        // ----------------------------------------------------------------
        // POST /rates
        // Crea un nuovo voto (o aggiorna quello esistente se l'utente
        // aveva già votato lo stesso brano/album).
        // Body esempio:
        // {
        //   "userId": "...",
        //   "albumSpotifyId": "5vkqYmiPBYLaalcmjujWxK",
        //   "rating": 9.5,
        //   "content": "Capolavoro assoluto",
        //   "moodTagId": 1
        // }
        // ----------------------------------------------------------------
        group.MapPost("/", async (CreateRateRequest request, AppDbContext db, CancellationToken ct) =>
        {
            // Il voto deve essere tra 0.0 e 10.0
            if (request.Rating < 0 || request.Rating > 10)
                return Results.BadRequest("Rating must be between 0.0 and 10.0.");

            // Bisogna specificare esattamente uno tra TrackSpotifyId e AlbumSpotifyId
            // Non si può votare un artista intero, solo brani o album
            var targetCount =
                (request.TrackSpotifyId is not null ? 1 : 0) +
                (request.AlbumSpotifyId is not null ? 1 : 0);

            if (targetCount != 1)
                return Results.BadRequest("Specify exactly one of: TrackSpotifyId, AlbumSpotifyId.");

            // Queste variabili conterranno l'ID interno (UUID del nostro DB)
            // del brano o album che si sta votando
            Guid? trackId = null;
            Guid? albumId = null;
            string targetName = ""; // Il nome del brano/album, usato nella risposta

            if (request.TrackSpotifyId is not null)
            {
                // Cerca la traccia nel nostro DB tramite lo spotify_id
                // Le tracce vengono salvate nel DB quando si chiama GET /artists/{id}/albums
                var track = await db.Tracks.FirstOrDefaultAsync(t => t.SpotifyId == request.TrackSpotifyId, ct);

                // Se la traccia non è ancora nel DB, restituisce errore 404
                // L'utente deve prima fare una chiamata a GET /artists per far salvare i dati
                if (track is null) return Results.NotFound($"Track '{request.TrackSpotifyId}' not found. Call GET /artists first to cache it.");

                trackId = track.Id;
                targetName = track.Title;
            }
            else if (request.AlbumSpotifyId is not null)
            {
                // Stessa cosa ma per gli album
                var album = await db.Albums.FirstOrDefaultAsync(a => a.SpotifyId == request.AlbumSpotifyId, ct);

                if (album is null) return Results.NotFound($"Album '{request.AlbumSpotifyId}' not found. Call GET /albums first to cache it.");

                albumId = album.Id;
                targetName = album.Title;
            }

            // Controlla se questo utente ha già votato questo brano/album
            // Se sì, aggiorna il voto invece di crearne uno nuovo (upsert)
            var existing = await db.Rates.FirstOrDefaultAsync(r =>
                r.UserId == request.UserId &&
                (trackId != null ? r.TrackId == trackId : r.AlbumId == albumId), ct);

            if (existing is not null)
            {
                // Voto già esistente: aggiorna i campi
                existing.Rating = request.Rating;
                existing.Content = request.Content;
                existing.MoodTagId = (short?)request.MoodTagId;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Primo voto: crea un nuovo record
                existing = new Rate
                {
                    UserId = request.UserId,
                    TrackId = trackId,
                    AlbumId = albumId,
                    Rating = request.Rating,
                    Content = request.Content,
                    MoodTagId = (short?)request.MoodTagId,
                };
                db.Rates.Add(existing); // Aggiunge il record alla lista da salvare
            }

            // Salva tutto sul DB (sia update che insert)
            await db.SaveChangesAsync(ct);

            // Recupera il nome del mood tag (es. "nostalgia") da mostrare nella risposta
            // Se non è stato specificato un mood tag, moodTag sarà null
            var moodTag = existing.MoodTagId.HasValue
                ? await db.MoodTags.FindAsync([existing.MoodTagId], ct)
                : null;

            // Costruisce la risposta da restituire al client
            var response = new RateResponse(
                Id: existing.Id,
                UserId: existing.UserId,
                TargetType: trackId.HasValue ? "track" : "album", // Indica se è un brano o un album
                TargetSpotifyId: request.TrackSpotifyId ?? request.AlbumSpotifyId!,
                TargetName: targetName,
                Rating: existing.Rating,
                Content: existing.Content,
                MoodTag: moodTag?.Name,
                CreatedAt: existing.CreatedAt
            );

            // Restituisce 201 Created con i dati del voto appena salvato
            return Results.Created($"/rates/{existing.Id}", response);
        })
        .WithName("CreateRate")
        .WithSummary("Rate a track or album (0.0 – 10.0). Updates the existing rate if already rated.");

        // ----------------------------------------------------------------
        // GET /rates/user/{userId}
        // Restituisce tutti i voti di un utente, dal più recente al più vecchio.
        // Supporta la paginazione tramite i parametri ?page=1&pageSize=20
        // Esempio: GET /rates/user/abc-123?page=2&pageSize=10
        // ----------------------------------------------------------------
        group.MapGet("/user/{userId:guid}", async (Guid userId, AppDbContext db, CancellationToken ct, int page = 1, int pageSize = 20) =>
        {
            var rates = await db.Rates
                .Where(r => r.UserId == userId)       // Solo i voti di questo utente
                .Include(r => r.Track)                 // Carica anche i dati della traccia collegata
                .Include(r => r.Album)                 // Carica anche i dati dell'album collegato
                .Include(r => r.MoodTag)               // Carica anche il mood tag
                .OrderByDescending(r => r.CreatedAt)   // Dal più recente al più vecchio
                .Skip((page - 1) * pageSize)           // Salta i risultati delle pagine precedenti
                .Take(pageSize)                        // Prendi solo il numero di risultati richiesto
                .ToListAsync(ct);

            // Trasforma ogni record del DB in un oggetto RateResponse da restituire al client
            var items = rates.Select(r => new RateResponse(
                Id: r.Id,
                UserId: r.UserId,
                TargetType: r.TrackId.HasValue ? "track" : "album",
                TargetSpotifyId: r.Track?.SpotifyId ?? r.Album?.SpotifyId ?? "",
                TargetName: r.Track?.Title ?? r.Album?.Title ?? "",
                Rating: r.Rating,
                Content: r.Content,
                MoodTag: r.MoodTag?.Name,
                CreatedAt: r.CreatedAt
            )).ToList();

            return Results.Ok(new { userId, page, pageSize, items });
        })
        .WithName("GetUserRates")
        .WithSummary("Get all rates for a user, paginated");
    }
}
