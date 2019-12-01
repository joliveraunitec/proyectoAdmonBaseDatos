using System;
using System.Collections.Generic;

namespace proyecto.Models
{
    public partial class Usuarios
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public string Correo { get; set; }
        public DateTime Creado { get; set; }
    }
}
