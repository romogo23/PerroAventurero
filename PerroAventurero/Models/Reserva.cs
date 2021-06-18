using System;
using System.Collections.Generic;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class Reserva
    {
        public int CodigoReserva { get; set; }
        public decimal? CodigoEvento { get; set; }
        public string CedulaCliente { get; set; }
        public string Cedula { get; set; }
        public short EntradasGenerales { get; set; }
        public short EntradasNinnos { get; set; }
        public DateTime FechaReserva { get; set; }
        public short Grupo { get; set; }
        public DateTime HoraEntrada { get; set; }
        public bool? EsAceptada { get; set; }
        public byte[] ComprobantePago { get; set; }
        public decimal PrecioTotal { get; set; }
        public bool? Asistencia { get; set; }

        public virtual Cliente CedulaClienteNavigation { get; set; }
        public virtual UsuarioAdministrador CedulaNavigation { get; set; }
        public virtual Evento CodigoEventoNavigation { get; set; }
    }
}
