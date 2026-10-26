using System;
using System.Collections.Generic;
using System.Text;

namespace DevWork.Shared.Models
{
    public class Publicacion
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string AreaSistemas { get; set; } = string.Empty;
        public string TipoOferta { get; set; } = string.Empty;
        public decimal PresupuestoOSalario { get; set; }
        public int UserId { get; set; }
    }
}
