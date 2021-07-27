using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerroAventurero.Models
{
    public class Reports
    {

        [Display(Name = "Nombre del evento")]
        public string NombreEvento { get; set; }

        [Display(Name = "Cantidad asistencia")]
        public int asistencia { get; set; }

        [Display(Name = "Cantidad reservas")]
        public int reservas { get; set; }
        public DateTime Fecha_Evento { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 999.99)]
        [Display(Name = "Porcentaje")]
        public decimal porcentaje { get; set; }

        [Display(Name = "Cédula")]
        public string cedulacliente { get; set; }

        [Display(Name = "Nombre de cliente")]
        public string NombreCliente { get; set; }

        [Display(Name = "Edad promedio")]
        public double edadPromedio { get; set; }

        [Display(Name = "Porcentaje de asistentes masculinos")]
        public decimal generoPromedioM { get; set; }

        [Display(Name = "Porcentaje de asistentes femeninos")]
        public decimal generoPromedioF { get; set; }

        [Display(Name = "Porcentaje de asistentes otros")]
        public decimal generoPromedioO { get; set; }

        [Display(Name = "Teléfono")]
        public int telefono{ get; set; }

        [Display(Name = "Asistencia a evento")]
        public int AttendanceEvent { get; set; }

    }
}
