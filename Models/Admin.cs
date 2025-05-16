using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace zapat.Models
{   
    [Table("t_Admin")]
    public class Admin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        public string? UserName { get; set; }

        public Decimal Total { get; set; }

        public DateTime Fecha { get; set; }

        public string? Status { get; set; }
    }
}