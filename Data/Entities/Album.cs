using System.ComponentModel.DataAnnotations.Schema;

namespace auuuxMS.Data.Entities;

[Table("albums")]
public class Album
{
    [Column("id")]           public Guid     Id          { get; set; } = Guid.NewGuid();
    [Column("spotify_id")]   public string   SpotifyId   { get; set; } = "";
    [Column("artist_id")]    public Guid     ArtistId    { get; set; }
    [Column("title")]        public string   Title       { get; set; } = "";
    [Column("release_date")] public DateOnly? ReleaseDate { get; set; }
    [Column("cover_url")]    public string?  CoverUrl    { get; set; }
    [Column("total_tracks")] public short?   TotalTracks { get; set; }
    [Column("album_type")]   public string?  AlbumType   { get; set; }
    [Column("created_at")]   public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;

    public Artist? Artist { get; set; }
}
