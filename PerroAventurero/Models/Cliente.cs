using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace PerroAventurero.Models
{
    public partial class Cliente
    {
        public Cliente()
        {
            Reservas = new HashSet<Reserva>();
        }

        [Display(Name = "Cédula")]
        [Required(ErrorMessage = "Su cédula es requerida")]
        [MaxLength(35, ErrorMessage = "Cédula muy larga")]
        public string CedulaCliente { get; set; }

        [Display(Name ="Nombre completo")]
        [Required(ErrorMessage = "Su nombre es requerido")]
        [MaxLength(250, ErrorMessage = "Nombre muy largo")]
        public string NombreCompleto { get; set; }

        [Display(Name = "Fecha nacimiento")]
        [Required(ErrorMessage = "Su fecha de nacimiento es requerida")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }


        [Display(Name = "Género")]
        [Required(ErrorMessage = "Su género es requerido")]
        public char Genero { get; set; }

        [Display(Name = "Télefono")]
        [Required(ErrorMessage = "Su número de teléfono es requerido")]
        [Range(00000000, 99999999, ErrorMessage = "Número inválido")]
        public int Telefono { get; set; }

        [Required(ErrorMessage = "Su correo electrónico es requerido")]
        [RegularExpression(@"^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$", ErrorMessage = "El correo no corresponde a uno válido")]
        public string Correo { get; set; }

        public bool? RecepcionAnuncios { get; set; }

        public virtual ICollection<Reserva> Reservas { get; set; }
    }
}
