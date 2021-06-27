using System;
using System.Collections.Generic;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class Afiliacion
    {
        public int Codigo { get; set; }
        public string CedulaCliente { get; set; }
        public string Cedula { get; set; }
        public DateTime? Fecha { get; set; }
        public byte[] ComprobantePago { get; set; }

        public virtual UsuarioComun CedulaClienteNavigation { get; set; }
        public virtual UsuarioAdministrador CedulaNavigation { get; set; }
    }
}
