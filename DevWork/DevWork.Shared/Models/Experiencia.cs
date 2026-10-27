using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DevWork.Shared.Models
{
    internal class Experiencia
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El ID del perfil profesional es obligatorio")]
        public int PerfilProfesionalId { get; set; }

        [Required(ErrorMessage = "El cargo es obligatorio")]
        [StringLength(100, ErrorMessage = "El cargo no puede tener más de 100 caracteres")]
        public string Cargo { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "La empresa no puede tener más de 100 caracteres")]
        [Required(ErrorMessage = "La empresa es obligatoria")]
        public string Empresa { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
