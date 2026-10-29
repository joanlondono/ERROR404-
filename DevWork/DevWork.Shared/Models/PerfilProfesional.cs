using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DevWork.Shared.Models
{
    public class PerfilProfesional
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El ID de usuario es obligatorio")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre completo no puede tener más de 100 caracteres")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El título profesional es obligatorio")]
        [StringLength(100, ErrorMessage = "El título profesional no puede tener más de 100 caracteres")]
        public string TituloProfesional { get; set; } = string.Empty;

        [Required(ErrorMessage = "La biografía es obligatoria")]
        [StringLength(500, ErrorMessage = "La biografía no puede tener más de 500 caracteres")]
        public string Biografia { get; set; } = string.Empty;

        [Required(ErrorMessage = "Las habilidades técnicas son obligatorias")]
        [StringLength(500, ErrorMessage = "Las habilidades técnicas no pueden tener más de 500 caracteres")]
        public string HabilidadesTecnicas { get; set; } = string.Empty;

        [Required(ErrorMessage = "El área de especialidad es obligatoria")]
        [StringLength(100, ErrorMessage = "El área de especialidad no puede tener más de 100 caracteres")]
        public string AreaEspecialidad { get; set; } = string.Empty;

        [Required(ErrorMessage = "La tarifa por hora es obligatoria")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TarifaHora { get; set; }

        [Required(ErrorMessage = "El enlace de GitHub es obligatorio")]
        [StringLength(200, ErrorMessage = "El enlace de GitHub no puede tener más de 200 caracteres")]
        [Url(ErrorMessage = "El enlace de GitHub no es válido")]
        public string EnlaceGitHub { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "El enlace de LinkedIn no puede tener más de 200 caracteres")]
        [Url(ErrorMessage = "El enlace de LinkedIn no es válido")]
        [Required(ErrorMessage = "El enlace de LinkedIn es obligatorio")]
        public string EnlaceLinkedIn { get; set; } = string.Empty;
    }
}
