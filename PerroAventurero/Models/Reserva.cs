using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class Reserva
    {
        public int CodigoReserva { get; set; }
        public int CodigoEvento { get; set; }
        public string CedulaCliente { get; set; }


        public string Cedula { get; set; }

        [Display(Name = "Entradas generales")]
        [Required(ErrorMessage = "Debe seleccionar la cantidad de entradas")]
        [Range(1, 100, ErrorMessage = "Debe ingresar un número válido")]
        public short EntradasGenerales { get; set; }

        [Display(Name = "Entradas niños")]
        [Range(0, 100, ErrorMessage = "Debe ingresar un número válido")]
        public short? EntradasNinnos { get; set; }


        public DateTime FechaReserva { get; set; }


        [Required(ErrorMessage = "Debe ingresar un grupo")]
        public short Grupo { get; set; }


        [Display(Name = "Hora de entrada")]
        [Required(ErrorMessage = "Debe seleccionar una hora de entraga")]
        [DataType(DataType.Time)]
        public DateTime HoraEntrada { get; set; }

        public bool? EsAceptada { get; set; }
        public byte[] ComprobantePago { get; set; }

        [Display(Name = "Precio total")]
        public decimal PrecioTotal { get; set; }
        public bool? Asistencia { get; set; }

        public virtual Cliente CedulaClienteNavigation { get; set; }
        public virtual UsuarioAdministrador CedulaNavigation { get; set; }
        public virtual Evento CodigoEventoNavigation { get; set; }
        public virtual ICollection<Acompannante> Acompannantes { get; set; }
    }
}
