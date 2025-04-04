using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace zapat.Models
{
    public class Producto
    {
        public string Nombre { get; set; } // Nombre del producto
        public decimal Precio { get; set; } // Precio del producto
        public string Imagen { get; set; } // URL de la imagen del producto
        public int Cantidad { get; set; } // Cantidad del producto
    }
}