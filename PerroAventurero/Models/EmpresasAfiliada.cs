using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class EmpresasAfiliada
    {
        public int CodigoEmpresa { get; set; }
        public string Cedula { get; set; }
        [Required(ErrorMessage = "El nombre de la empresa es requerido")]
        [StringLength(60, MinimumLength = 3)]
        [Display(Name = "Nombre de empresa")]
        public string NombreEmpresa { get; set; }
        [Required(ErrorMessage = "El correo de la empresa es requerido")]
        [RegularExpression(@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$", ErrorMessage = "El correo no corresponde a uno válido")]
        public string Correo { get; set; }
        public byte[] Logo { get; set; }
        [Required(ErrorMessage = "La categoría es requerida")]
        public string Categoria { get; set; }

        [Display(Name = "Télefono")]
        [Required(ErrorMessage = "Su télefono requerido")]
        public int? Telefono { get; set; }

        public string NombreContacto { get; set; }

        public virtual UsuarioAdministrador CedulaNavigation { get; set; }
    }
}
