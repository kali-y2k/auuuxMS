# auuux — Overview del progetto

## Struttura del codice

### `Program.cs`
Punto di avvio. Registra tutti i servizi (SpotifyTokenProvider, SpotifyService) e monta tutti gli endpoint. Se aggiungi un nuovo gruppo di endpoint, lo registri qui.

---

### `Endpoints/`
Ogni file gestisce le route HTTP di una risorsa. La logica è minima: validazione input, chiamata al service, risposta.

| File | Route principali |
|---|---|
| `ArtistsEndpoints.cs` | `GET /artists/{id}`, `GET /artists/{id}/albums` |
| `AlbumsEndpoints.cs` | `GET /albums/{id}` |
| `SearchEndpoints.cs` | `GET /search?q=...` |
| `ConnectsEndpoints.cs` | `POST /connects`, `GET /connects/user/{id}` |
| `RatesEndpoints.cs` | `POST /rates`, `GET /rates/user/{id}` |
| `MomentsEndpoints.cs` | `POST /moments`, `GET /moments/live` |
| `FeedEndpoints.cs` | `GET /feed`, `GET /feed/friends` |
| `UsersEndpoints.cs` | profilo, diary, wrapped, myvinyl, tops, follow/unfollow |

Gli endpoint di Artists/Albums/Search sono già funzionanti (chiamano Spotify). Gli altri (Connects, Rates, Moments, Feed, Users) hanno la struttura e la validazione pronte ma hanno un `// TODO` dove va la query al DB — vanno completati quando si collega Supabase.

---

### `Services/`
Contiene la logica di business.

- **`SpotifyService.cs`** — fa le chiamate effettive all'API Spotify. Qui dentro ci sono i metodi `GetArtistAsync`, `GetAlbumAsync`, `GetArtistAlbumsAsync`, `SearchAsync`. Se vuoi aggiungere una nuova chiamata Spotify (es. get tracce di un album), la aggiungi qui.
- **`Interfaces/ISpotifyService.cs`** — definisce il contratto di SpotifyService. Serve per poter testare il codice senza fare chiamate reali a Spotify.

---

### `Infrastructure/`
Roba tecnica di supporto, separata dalla logica.

- **`SpotifyTokenProvider.cs`** — gestisce il token OAuth2 di Spotify. Ottiene il token con le credenziali (Client Credentials flow), lo tiene in cache finché non scade, e lo rinnova in automatico. Il resto del codice non deve pensarci.

---

### `Models/`
Strutture dati. Divise in due cartelle:

- **`Models/Spotify/`** — i dati così come arrivano da Spotify, mappati in record C#: `SpotifyArtist`, `SpotifyAlbum`, `SpotifySearchResult`, ecc.
- **`Models/Domain/`** — i dati dell'app: request e response per connects, rates, moments, feed, profilo utente, ecc.

---

### `Data/`
Cartella vuota per ora. Qui andrà il `DbContext` di Entity Framework quando si collega Supabase.

---

### `Database/schema.sql`
Lo script SQL da eseguire su Supabase per creare tutte le tabelle. Va rieseguito ogni volta che si modifica la struttura del DB.

---

## Struttura del Database

### Tabelle di cache Spotify
Non si vuole richiamare Spotify ogni volta, quindi la prima volta che un artista/album/traccia viene usato nell'app, viene salvato localmente.

- **`artists`** — artisti. Collegati agli album.
- **`albums`** — album. Ogni album appartiene a un artista.
- **`tracks`** — tracce. Ogni traccia appartiene a un album.

Tutte e tre hanno un campo `spotify_id` (unique) che è il punto di collegamento con l'API Spotify.

---

### Tabelle utenti e social

- **`users`** — gli utenti dell'app: username, email, avatar, bio.
- **`follows`** — chi segue chi. È una tabella di relazione con `follower_id` e `following_id`. C'è un constraint che impedisce di seguire se stessi.
- **`mood_tags`** — lista fissa di tag mood (nostalgia, melancholy, late night…). Già popolata dallo script.

---

### Tabelle di attività
Le tre azioni principali che un utente può fare:

- **`connects`** — l'utente "connette" un brano, album o artista, con testo e mood opzionali. Un constraint garantisce che punti a esattamente uno dei tre.
- **`rates`** — l'utente vota da 0.0 a 10.0. Può votare tracce o album (non artisti). Un utente può votare lo stesso elemento una sola volta (ma può aggiornare il voto).
- **`moments`** — post live legati a una traccia, con una finestra di 27 minuti (`expires_at`). Dopo quella scadenza sono "archivio".

---

### Tabelle profilo

- **`my_vinyl`** + **`my_vinyl_tracks`** — la playlist identitaria dell'utente, max 12 tracce ordinate per `position`. `my_vinyl` tiene il conteggio degli swap rimasti.
- **`user_top_artists`** — i top 3 artisti pinnati sul profilo (position 1–3).
- **`user_top_albums`** — i top 3 album pinnati sul profilo (position 1–3).

---

## Flusso di una richiesta Spotify

```
HTTP Request → Endpoint → ISpotifyService → SpotifyService
                                                  ↓
                                        SpotifyTokenProvider (token OAuth2)
                                                  ↓
                                          Spotify API (https://api.spotify.com/v1/)
                                                  ↓
                                        risposta JSON mappata in record C#
                                                  ↓
                               HTTP Response (JSON automatico con ASP.NET)
```

## Prossimi passi

1. Collegare Supabase: aggiungere la connection string in `appsettings.Development.json` e creare il `DbContext` in `Data/`.
2. Completare i `// TODO` negli endpoint con le query EF Core al DB.
3. Aggiungere autenticazione utenti.
