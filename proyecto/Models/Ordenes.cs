using System;
using System.Collections.Generic;

namespace proyecto.Models
{
    public partial class Ordenes
    {
        public int Id { get; set; }
        public int Cliente { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string Descripccion { get; set; }
        public string Estado { get; set; }
        public DateTime Creado { get; set; }

        public virtual Clientes ClienteNavigation { get; set; }
    }
}
