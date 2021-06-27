using System;
using System.Collections.Generic;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class Cliente
    {
        public Cliente()
        {
            Reservas = new HashSet<Reserva>();
        }

        public string CedulaCliente { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public char Genero { get; set; }
        public int Telefono { get; set; }
        public string Correo { get; set; }
        public bool? RecepcionAnuncios { get; set; }

        public virtual ICollection<Reserva> Reservas { get; set; }
    }
}
