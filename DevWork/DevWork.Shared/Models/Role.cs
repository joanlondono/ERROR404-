 using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DevWork.Shared.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre del rol no puede tener más de 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;
    }
}
