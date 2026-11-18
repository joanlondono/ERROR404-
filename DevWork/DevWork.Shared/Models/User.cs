using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DevWork.Shared.Models
{
    public class User : IdentityUser<int>
    {

        [MaxLength(25, ErrorMessage = "La contraseña no puede tener más de 25 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debes seleccionar un rol válido")]
        public int RoleId { get; set; }

        [Display(Name = "Foto")]
        public string? Photo { get; set; }
    }

}
