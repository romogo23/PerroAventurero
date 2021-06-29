using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerroAventurero.Models
{
    public class ReservaCliente
    {
        public Cliente cliente { get; set; }
        public Reserva reserva { get; set; }

        public List<Acompannante> acompannantes { set; get; }

        public ReservaCliente() {
            acompannantes = new List<Acompannante>();
        }
    }
}
