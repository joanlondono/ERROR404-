using System;
using System.Collections.Generic;
using System.Text;

namespace DevWork.Shared.Models
{
    internal class Educacion
    {
        public int Id { get; set; }
        public int PerfilProfesionalId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Institucion { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
