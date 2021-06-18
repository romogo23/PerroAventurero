using System;
using System.Collections.Generic;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class Acompannante
    {
        public int? CodigoReserva { get; set; }
        public string Genero { get; set; }
        public short Edad { get; set; }
        public bool? Asistencia { get; set; }
        public int Codigotempo { get; set; }

        public virtual Reserva CodigoReservaNavigation { get; set; }
    }
}
