using System.ComponentModel.DataAnnotations.Schema;

namespace auuuxMS.Data.Entities;

[Table("user_top_artists")]
public class UserTopArtist
{
    [Column("user_id")]   public Guid  UserId   { get; set; }
    [Column("artist_id")] public Guid  ArtistId { get; set; }
    [Column("position")]  public short Position { get; set; }

    public User?   User   { get; set; }
    public Artist? Artist { get; set; }
}

[Table("user_top_albums")]
public class UserTopAlbum
{
    [Column("user_id")]  public Guid  UserId   { get; set; }
    [Column("album_id")] public Guid  AlbumId  { get; set; }
    [Column("position")] public short Position { get; set; }

    public User?  User  { get; set; }
    public Album? Album { get; set; }
}
