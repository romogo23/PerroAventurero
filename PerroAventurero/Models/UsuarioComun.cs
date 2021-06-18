using System;
using System.Collections.Generic;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class UsuarioComun
    {
        public UsuarioComun()
        {
            Afiliacions = new HashSet<Afiliacion>();
        }

        public string CedulaCliente { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaCambioC { get; set; }
        public byte[] Foto { get; set; }
        public short? CodigoTemporal { get; set; }
        public string Contrasenna { get; set; }

        public virtual ICollection<Afiliacion> Afiliacions { get; set; }
    }
}
