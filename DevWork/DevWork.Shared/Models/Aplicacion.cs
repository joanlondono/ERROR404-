using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DevWork.Shared.Models
{
    public class Aplicacion
    {
        public int Id { get; set; }

        // ID de la publicación a la que se postula
        [Range(1, int.MaxValue, ErrorMessage = "El ID de la publicación es obligatorio")]
        [Required(ErrorMessage = "El ID de la publicación es obligatorio")]
        public int PublicacionId { get; set; }

        // ID del perfil profesional o usuario que se postula
        [Range(1, int.MaxValue, ErrorMessage = "El ID del perfil profesional es obligatorio")]
        [Required(ErrorMessage = "El ID del perfil profesional es obligatorio")]
        public int PerfilProfesionalId { get; set; }

        // Mensaje o propuesta que escribe el desarrollador
        [StringLength(200, ErrorMessage = "El mensaje de la propuesta no puede tener más de 200 caracteres")]
        [Required(ErrorMessage = "El mensaje de la propuesta es obligatorio")]
        public string PropuestaMensaje { get; set; } = string.Empty;

        // Estado de la postulación (ej: "Pendiente", "Aceptada", "Rechazada")}
        [StringLength(50, ErrorMessage = "El estado de la postulación no puede tener más de 50 caracteres")]
        [Required(ErrorMessage = "El estado de la postulación es obligatorio")]
        public string Estado { get; set; } = "Pendiente";

        // Fecha en que se envió la postulación
        [Required(ErrorMessage = "La fecha de postulación es obligatoria")]
        [DataType(DataType.Date, ErrorMessage = "La fecha de postulación no es válida")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaPostulacion { get; set; } = DateTime.Now;
    }
}
