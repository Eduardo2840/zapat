using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
namespace zapat.Models
{
    [Table("t_contacto")]
    public class Contacto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Nombres son obligatorios.")]
        public string? Nombres { get; set; }
        [NotNull]
        public string? Email { get; set; }
        [NotNull]
        public string? Mensaje { get; set; }

        public string? Etiqueta { get; set; }

        public float Puntuacion { get; set; }
        
    }
}