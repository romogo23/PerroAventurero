using System;
using System.Collections.Generic;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class UsuarioAdministrador
    {
        public UsuarioAdministrador()
        {
            Afiliacions = new HashSet<Afiliacion>();
            EmpresasAfiliada = new HashSet<EmpresasAfiliada>();
            Eventos = new HashSet<Evento>();
            Reservas = new HashSet<Reserva>();
        }

        public string Cedula { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Genero { get; set; }
        public int? Telefono { get; set; }
        public string Correo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaCambioC { get; set; }
        public byte[] Foto { get; set; }
        public short? CodigoTemporal { get; set; }
        public string Contrasenna { get; set; }

        public virtual ICollection<Afiliacion> Afiliacions { get; set; }
        public virtual ICollection<EmpresasAfiliada> EmpresasAfiliada { get; set; }
        public virtual ICollection<Evento> Eventos { get; set; }
        public virtual ICollection<Reserva> Reservas { get; set; }
    }
}
