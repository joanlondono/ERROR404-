using System;
using System.Collections.Generic;
using System.Text;

namespace DevWork.Shared.Models
{
    public class Certificacion
    {
        public int Id { get; set; }
        public int PerfilProfesionalId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string EnteEmisor { get; set; } = string.Empty;
        public DateTime FechaObtencion { get; set; }
        public string? UrlCredencial { get; set; }
    }
}
