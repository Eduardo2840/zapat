using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace zapat.Models
{
    [Table("t_preorden")]
    public class PreOrden
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? UserName { get; set; }
        public Producto? Producto { get; set; }
        public int Cantidad { get; set; }
        [NotNull]
        public Decimal Precio { get; set; }
        public string Status { get; set; } = "PENDIENTE";
        
    }
}