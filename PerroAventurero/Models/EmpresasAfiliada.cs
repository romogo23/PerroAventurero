using System;
using System.Collections.Generic;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class EmpresasAfiliada
    {
        public int CodigoEmpresa { get; set; }
        public string Cedula { get; set; }
        public string NombreEmpresa { get; set; }
        public string Correo { get; set; }
        public byte[] Logo { get; set; }
        public string Categoria { get; set; }
        public int? Telefono { get; set; }

        public virtual UsuarioAdministrador CedulaNavigation { get; set; }
    }
}
