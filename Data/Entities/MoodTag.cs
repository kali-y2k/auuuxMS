using System.ComponentModel.DataAnnotations.Schema;

namespace auuuxMS.Data.Entities;

[Table("mood_tags")]
public class MoodTag
{
    [Column("id")]   public short  Id   { get; set; }
    [Column("name")] public string Name { get; set; } = "";
}
