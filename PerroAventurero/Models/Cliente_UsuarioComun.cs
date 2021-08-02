using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerroAventurero.Models
{
    public class Cliente_UsuarioComun
    {
        
        public virtual Cliente Cliente{ get; set; }

        public virtual UsuarioComun UsuarioComun { get; set; }
    }
}
