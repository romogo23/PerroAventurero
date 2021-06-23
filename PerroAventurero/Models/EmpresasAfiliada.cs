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
        [Required(ErrorMessage = "Se requiere un nombre de empresa")]
        [StringLength(60, MinimumLength = 3)]
        [Display(Name = "Nombre de empresa")]
        public string NombreEmpresa { get; set; }
        [Required(ErrorMessage = "Se requiere un correo de empresa")]
        [RegularExpression(@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$", ErrorMessage = "El correo no corresponde a uno válido")]
        public string Correo { get; set; }
        public byte[] Logo { get; set; }
        [Required(ErrorMessage = "Se requiere una categoría")]
        public string Categoria { get; set; }
        public int? Telefono { get; set; }

        public virtual UsuarioAdministrador CedulaNavigation { get; set; }
    }
}
