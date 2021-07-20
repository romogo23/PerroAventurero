using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerroAventurero.Models
{
    public class Reports
    {
        public string NombreEvento { get; set; }
        public int asistencia { get; set; }
        public int reservas { get; set; }
        public DateTime Fecha_Evento { get; set; }

        [RegularExpression(@"^\d+\.\d{0,2}$")]
        [Range(0, 999.99)]
        public decimal porcentaje { get; set; }

        public string cedulacliente { get; set; }

        public string NombreCliente { get; set; }

        public double edadPromedio { get; set; }

        public double generoPromedioM { get; set; }
        public double generoPromedioF { get; set; }
        public double AverageGemderO { get; set; }
        public int telefono{ get; set; }

        public int AttendanceEvent { get; set; }

    }
}
