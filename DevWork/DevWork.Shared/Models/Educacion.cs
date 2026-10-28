using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DevWork.Shared.Models
{
    public class Educacion
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El ID del perfil profesional es obligatorio")]
        public int PerfilProfesionalId { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(100, ErrorMessage = "El título no puede tener más de 100 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La institución es obligatoria")]
        [StringLength(100, ErrorMessage = "La institución no puede tener más de 100 caracteres")]
        public string Institucion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
