using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace zapat.Models
{
    [Table("t_Mujer")]
    public class ProMujer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [NotNull]
        public string? Name { get; set; }
        [NotNull]
        public Decimal Price { get; set; }
        [NotNull]
        public string? Status { get; set; }
        [NotNull]
        public string? ImageURL { get; set; }
    }
}