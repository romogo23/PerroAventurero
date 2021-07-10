﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class Reserva
    {
        public Reserva()
        {
            Acompannantes = new List<Acompannante>();
        }

        public int CodigoReserva { get; set; }
        public int CodigoEvento { get; set; }
        public string CedulaCliente { get; set; }
        public string Cedula { get; set; }
        [Required(ErrorMessage = "Debe seleccionar la cantidad de entradas")]
        [Range(1, 100, ErrorMessage = "Debe ingresar un número válido")]
        public short EntradasGenerales { get; set; }

        [Range(1, 100, ErrorMessage = "Debe ingresar un número válido")]
        public short? EntradasNinnos { get; set; }

        public DateTime FechaReserva { get; set; }

        [Required(ErrorMessage = "Debe ingresar un grupo")]
        public short Grupo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una hora de entraga")]
        [DataType(DataType.Time)]
        public DateTime HoraEntrada { get; set; }

        public bool? EsAceptada { get; set; }
        public byte[] ComprobantePago { get; set; }
        public decimal PrecioTotal { get; set; }
        public bool? Asistencia { get; set; }

        public virtual Cliente CedulaClienteNavigation { get; set; }
        public virtual UsuarioAdministrador CedulaNavigation { get; set; }
        public virtual Evento CodigoEventoNavigation { get; set; }
        [NotMapped]
        public virtual List<Acompannante> Acompannantes { get; set; }
    }
}
