using System;
using System.Collections.Generic;

namespace proyecto.Models
{
    public partial class Clientes
    {
        public Clientes()
        {
            Ordenes = new HashSet<Ordenes>();
        }

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Rtn { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }

        public virtual ICollection<Ordenes> Ordenes { get; set; }
    }
}
