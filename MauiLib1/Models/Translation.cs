using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauiLib1.Models;

[Table("Translation")]
public class Translation
{
    [Key]
    [Column("id")]
    public long id { get; set; }

    [Column("translation")]
    public string? translation { get; set; }

    public ICollection<Word> words { get; set; } = new List<Word>();
}
