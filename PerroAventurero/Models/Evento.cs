using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class Evento
    {
        public Evento()
        {
            Reservas = new HashSet<Reserva>();
        }

        public int CodigoEvento { get; set; }
        public string Cedula { get; set; }
        
        [Required(ErrorMessage ="El nombre del evento es requerido")]
        [MaxLength(80)]
        public string NombreEvento { get; set; }


        [Required(ErrorMessage = "El lugar del evento es requerido")]
        [MaxLength(100)]
        public string Lugar { get; set; }


        [Required(ErrorMessage = "La dirección del evento es requerida")]
        [MaxLength(200)]
        public string Direccion { get; set; }


        [Required(ErrorMessage = "La fecha del evento es requerida")]
        public DateTime Fecha { get; set; }


        [Required(ErrorMessage = "El precio de las entradas generales del evento es requerido")]
        public decimal PrecioGeneral { get; set; }

        [Required(ErrorMessage = "El precio de las entradas de niños del evento es requerido")]
        public decimal PrecioNinno { get; set; }

        [Required(ErrorMessage = "La cantidad del aforo del evento es requerida")]
        public int CantidadAforo { get; set; }

        [Required(ErrorMessage = "La hora de inicio de entrada al evento es requerida")]
        public DateTime HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de final de entrada al evento es requerida")]
        public DateTime HoraFinal { get; set; }

        public bool EnvioAnuncios { get; set; }
        public string Comentarios { get; set; }

        public virtual UsuarioAdministrador CedulaNavigation { get; set; }
        public virtual ICollection<Reserva> Reservas { get; set; }
    }
}
