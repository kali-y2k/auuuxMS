using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace auuuxMS.Data.Entities;

[Table("users")]
public class User
{
    [Column("id")]       public Guid   Id        { get; set; } = Guid.NewGuid();
    [Column("username")] public string Username  { get; set; } = "";
    [Column("email")]    public string Email     { get; set; } = "";
    [Column("avatar_url")] public string? AvatarUrl { get; set; }
    [Column("bio")]      public string? Bio      { get; set; }
    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
