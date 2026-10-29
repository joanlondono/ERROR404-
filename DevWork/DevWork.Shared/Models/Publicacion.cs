using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DevWork.Shared.Models
{
    public class Publicacion
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(100, ErrorMessage = "El título no puede tener más de 100 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [MinLength(20, ErrorMessage = "La descripción debe tener al menos 20 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El área de sistemas es obligatoria")]
        [StringLength(100, ErrorMessage = "El área de sistemas no puede tener más de 100 caracteres")]
        public string AreaSistemas { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de oferta es obligatorio")]
        [StringLength(100, ErrorMessage = "El tipo de oferta no puede tener más de 100 caracteres")]
        public string TipoOferta { get; set; } = string.Empty;

        [Required(ErrorMessage = "El presupuesto o salario es obligatorio")]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0.01,100000000, ErrorMessage = "El presupuesto o salario debe ser un valor positivo")]
        public decimal PresupuestoOSalario { get; set; }

        [Required(ErrorMessage = "El ID de usuario es obligatorio")]
        public int UserId { get; set; }
    }
}
