using System.ComponentModel.DataAnnotations.Schema;

namespace auuuxMS.Data.Entities;

[Table("follows")]
public class Follow
{
    [Column("follower_id")]  public Guid     FollowerId  { get; set; }
    [Column("following_id")] public Guid     FollowingId { get; set; }
    [Column("created_at")]   public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;

    public User? Follower  { get; set; }
    public User? Following { get; set; }
}
