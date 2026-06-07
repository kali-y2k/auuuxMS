using System.ComponentModel.DataAnnotations.Schema;

namespace auuuxMS.Data.Entities;

[Table("moments")]
public class Moment
{
    [Column("id")]          public Guid     Id        { get; set; } = Guid.NewGuid();
    [Column("user_id")]     public Guid?    UserId    { get; set; }
    [Column("track_id")]    public Guid     TrackId   { get; set; }
    [Column("content")]     public string?  Content   { get; set; }
    [Column("mood_tag_id")] public short?   MoodTagId { get; set; }
    [Column("created_at")]  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("expires_at")]  public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddMinutes(27);

    public User?    User    { get; set; }
    public Track?   Track   { get; set; }
    public MoodTag? MoodTag { get; set; }
}
