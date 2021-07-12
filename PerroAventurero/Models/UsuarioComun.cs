using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class UsuarioComun
    {
        public UsuarioComun()
        {
            Afiliacions = new HashSet<Afiliacion>();
        }

        [Display(Name = "Cédula")]
        [Required(ErrorMessage = "Su número cédula es requerido")]
        [MaxLength(35, ErrorMessage = "Cédula muy larga")]
        public string CedulaCliente { get; set; }

        [Display(Name = "Descripción")]
        [MaxLength(250, ErrorMessage = "Descripción muy larga")]
        public string Descripcion { get; set; }
        public DateTime? FechaCambioC { get; set; }
        public byte[] Foto { get; set; } 
        public short? CodigoTemporal { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "La contraseña es requerida")]
        [MaxLength(11, ErrorMessage = "Constraseña debe tener máximo 11 caracteres")]
        [MinLength(8, ErrorMessage = "Constraseña debe tener mínimo 8 caracteres")]
        public string Contrasenna { get; set; }


        public virtual ICollection<Afiliacion> Afiliacions { get; set; }
    }
}
