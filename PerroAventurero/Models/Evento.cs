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
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }


        [Required(ErrorMessage = "El precio de las entradas generales del evento es requerido")]
        [Range(1, int.MaxValue, ErrorMessage ="El precio de las entradas generales debe ser positivo")]
        public decimal PrecioGeneral { get; set; }

        [Required(ErrorMessage = "El precio de las entradas de niños del evento es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El precio de las entradas de niños debe ser positivo")]
        public decimal PrecioNinno { get; set; }

        [Required(ErrorMessage = "La cantidad del aforo del evento es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de aforo debe ser un valor positivo")]
        public int CantidadAforo { get; set; }

        [Required(ErrorMessage = "La cantidad de grupos del evento es requerida")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad de grupos debe ser un valor positivo")]
        public int CantidadGrupos { get; set; }

        [Required(ErrorMessage = "La hora de inicio de entrada al evento es requerida")]
        [DataType(DataType.Time)]
        public DateTime HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de final de entrada al evento es requerida")]
        [DataType(DataType.Time)]
        public DateTime HoraFinal { get; set; }

        public bool EnvioAnuncios { get; set; }
        public string Comentarios { get; set; }

        public byte[] Imagen { get; set; }

        public virtual UsuarioAdministrador CedulaNavigation { get; set; }
        public virtual ICollection<Reserva> Reservas { get; set; }
    }
}
