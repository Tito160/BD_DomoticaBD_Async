using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BD_DomoticaBD_Async.mvc.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Contrasenia { get; set; } = "";
        public string Telefono { get; set; } = "";
    }
}
