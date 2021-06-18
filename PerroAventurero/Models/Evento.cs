using System;
using System.Collections.Generic;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class Evento
    {
        public Evento()
        {
            Reservas = new HashSet<Reserva>();
        }

        public decimal CodigoEvento { get; set; }
        public string Cedula { get; set; }
        public string NombreEvento { get; set; }
        public string Lugar { get; set; }
        public string Dirreccion { get; set; }
        public DateTime? Fecha { get; set; }
        public decimal? PrecioGeneral { get; set; }
        public decimal? PrecioNinno { get; set; }
        public int? CantidadAforo { get; set; }
        public DateTime? HoraInicio { get; set; }
        public DateTime? HoraFinal { get; set; }
        public bool? EnvioAnuncios { get; set; }
        public string Comentarios { get; set; }

        public virtual UsuarioAdministrador CedulaNavigation { get; set; }
        public virtual ICollection<Reserva> Reservas { get; set; }
    }
}
