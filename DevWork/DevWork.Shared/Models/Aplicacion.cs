using System;
using System.Collections.Generic;
using System.Text;

namespace DevWork.Shared.Models
{
    internal class Aplicacion
    {
        public int Id { get; set; }

        // ID de la publicación a la que se postula
        public int PublicacionId { get; set; }

        // ID del perfil profesional o usuario que se postula
        public int PerfilProfesionalId { get; set; }

        // Mensaje o propuesta que escribe el desarrollador
        public string PropuestaMensaje { get; set; } = string.Empty;

        // Estado de la postulación (ej: "Pendiente", "Aceptada", "Rechazada")
        public string Estado { get; set; } = "Pendiente";

        // Fecha en que se envió la postulación
        public DateTime FechaPostulacion { get; set; } = DateTime.Now;
    }
}
