using System.ComponentModel.DataAnnotations.Schema;

namespace auuuxMS.Data.Entities;

[Table("my_vinyl")]
public class MyVinyl
{
    [Column("user_id")]         public Guid     UserId         { get; set; }
    [Column("swaps_remaining")] public short    SwapsRemaining { get; set; } = 2;
    [Column("updated_at")]      public DateTime UpdatedAt      { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public ICollection<MyVinylTrack> Tracks { get; set; } = [];
}

[Table("my_vinyl_tracks")]
public class MyVinylTrack
{
    [Column("user_id")]  public Guid  UserId   { get; set; }
    [Column("track_id")] public Guid  TrackId  { get; set; }
    [Column("position")] public short Position { get; set; }

    public Track?   Track   { get; set; }
    public MyVinyl? MyVinyl { get; set; }
}
