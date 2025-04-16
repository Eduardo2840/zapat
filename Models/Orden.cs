using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace zapat.Models
{
    [Table("t_order")]
    public class Orden
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        public string? UserID { get; set; }
        public Decimal Total { get; set; }
        public DateTime Fecha { get; set; }
        public string? Status { get; set; }
    }
}