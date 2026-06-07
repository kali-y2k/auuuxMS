using System.ComponentModel.DataAnnotations.Schema;

namespace auuuxMS.Data.Entities;

[Table("rates")]
public class Rate
{
    [Column("id")]          public Guid     Id        { get; set; } = Guid.NewGuid();
    [Column("user_id")]     public Guid?    UserId    { get; set; }
    [Column("track_id")]    public Guid?    TrackId   { get; set; }
    [Column("album_id")]    public Guid?    AlbumId   { get; set; }
    [Column("rating")]      public decimal  Rating    { get; set; }
    [Column("content")]     public string?  Content   { get; set; }
    [Column("mood_tag_id")] public short?   MoodTagId { get; set; }
    [Column("created_at")]  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")]  public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User?    User    { get; set; }
    public Track?   Track   { get; set; }
    public Album?   Album   { get; set; }
    public MoodTag? MoodTag { get; set; }
}
