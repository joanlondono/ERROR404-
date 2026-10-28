using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DevWork.Shared.Models
{
    public class Certificacion
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El ID del perfil profesional es obligatorio")]
        public int PerfilProfesionalId { get; set; }

        [Required(ErrorMessage = "El nombre de la certificación es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre de la certificación no puede tener más de 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "El ente emisor no puede tener más de 100 caracteres")]
        public string EnteEmisor { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de obtención es obligatoria")]
        [DataType(DataType.Date, ErrorMessage = "La fecha de obtención no es válida")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaObtencion { get; set; }

        [StringLength(200, ErrorMessage = "La URL de la credencial no puede tener más de 200 caracteres")]
        [Url(ErrorMessage = "La URL de la credencial no es válida")]
        [Required(ErrorMessage = "La URL de la credencial es obligatoria")]
        public string? UrlCredencial { get; set; }
    }
}
