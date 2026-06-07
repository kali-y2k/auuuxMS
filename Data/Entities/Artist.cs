using System.ComponentModel.DataAnnotations.Schema;

namespace auuuxMS.Data.Entities;

[Table("artists")]
public class Artist
{
    [Column("id")]          public Guid     Id        { get; set; } = Guid.NewGuid();
    [Column("spotify_id")]  public string   SpotifyId { get; set; } = "";
    [Column("name")]        public string   Name      { get; set; } = "";
    [Column("image_url")]   public string?  ImageUrl  { get; set; }
    [Column("genres")]      public string[] Genres    { get; set; } = [];
    [Column("created_at")]  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
