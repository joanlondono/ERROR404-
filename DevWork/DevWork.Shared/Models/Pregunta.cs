using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace DevWork.Shared.Models
{
    public class Pregunta
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El ID de la publicación es obligatorio")]
        [Required(ErrorMessage = "El ID de la publicación es obligatorio")]
        public int PublicacionId { get; set; }

        [Required(ErrorMessage = "El texto de la pregunta es obligatorio")]
        [StringLength(200, ErrorMessage = "El texto de la pregunta no puede tener más de 200 caracteres")]
        public string TextoPregunta { get; set; } = string.Empty;
    }
}
