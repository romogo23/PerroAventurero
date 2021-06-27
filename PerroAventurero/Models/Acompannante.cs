using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class Acompannante
    {
        public int CodigoReserva { get; set; }
        public char Genero { get; set; }
        public short Edad { get; set; }
        public bool? Asistencia { get; set; }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Codigo { get; set; }

        public virtual Reserva CodigoReservaNavigation { get; set; }
    }
}
