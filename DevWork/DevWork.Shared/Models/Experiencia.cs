using System;
using System.Collections.Generic;
using System.Text;

namespace DevWork.Shared.Models
{
    public class Experiencia
    {
        public int Id { get; set; }
        public int PerfilProfesionalId { get; set; }
        public string Cargo { get; set; } = string.Empty;
        public string Empresa { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
