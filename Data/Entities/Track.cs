using System.ComponentModel.DataAnnotations.Schema;

namespace auuuxMS.Data.Entities;

[Table("tracks")]
public class Track
{
    [Column("id")]           public Guid     Id          { get; set; } = Guid.NewGuid();
    [Column("spotify_id")]   public string   SpotifyId   { get; set; } = "";
    [Column("album_id")]     public Guid     AlbumId     { get; set; }
    [Column("title")]        public string   Title       { get; set; } = "";
    [Column("track_number")] public short?   TrackNumber { get; set; }
    [Column("duration_ms")]  public int?     DurationMs  { get; set; }
    [Column("created_at")]   public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;

    public Album? Album { get; set; }
}
