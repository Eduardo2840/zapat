using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace zapat.Models
{
    [Table("t_register")]
    public class Register
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Nombres son obligatorios.")]
        public string? Nombre { get; set; }
        [NotNull]
        public string? Apellido { get; set;}
        [NotNull]
        public string? Correo { get; set;}
        [NotNull]
        public string? Contra { get; set;}
       
        
    }
}