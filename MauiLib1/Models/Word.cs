using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiLib1.Models
{
    [Table("Words")]
    public class Word
    {
        [Key]
        [Column("id")]
        public long id { get; set; }


        [Column("word")]
        public string? word { get; set; }

        public ICollection<Translation> translations { get; set; } = new List<Translation>();
    }
}
