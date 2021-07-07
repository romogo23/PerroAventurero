using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerroAventurero.Models
{
    public class Cliente_UsuarioComun
    {
        public Cliente_UsuarioComun(UsuarioComun usuarioComun, Cliente cliente)
        {
            UsuarioComun = usuarioComun;
            Cliente = cliente;
        }

        public Cliente Cliente { get; set; }

        public UsuarioComun UsuarioComun { get; set; }
    }
}
