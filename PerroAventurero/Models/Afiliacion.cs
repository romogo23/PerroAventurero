using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class Afiliacion
    {
        public int Codigo { get; set; }

        [Display(Name = "Cédula")]
        [Required(ErrorMessage = "Su cédula es requerido")]
        [MaxLength(35, ErrorMessage = "Cédula muy larga")]
        public string CedulaCliente { get; set; }

        [Display(Name = "Cédula")]
        [Required(ErrorMessage = "Su cédula es requerido")]
        [MaxLength(35, ErrorMessage = "Cédula muy larga")]
        public string Cedula { get; set; }
      
        public DateTime? Fecha { get; set; }

        [Display(Name = "Comprobante de pago")]
        public byte[] ComprobantePago { get; set; }

        public virtual UsuarioComun CedulaClienteNavigation { get; set; }
        public virtual UsuarioAdministrador CedulaNavigation { get; set; }
    }
}
