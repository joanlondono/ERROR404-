using System;
using System.Collections.Generic;
using System.Text;

namespace DevWork.Shared.Models
{
    internal class PerfilProfesional
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TituloProfesional { get; set; } = string.Empty;
        public string Biografia { get; set; } = string.Empty;
        public string HabilidadesTecnicas { get; set; } = string.Empty;
        public decimal AreaEspecialidad { get; set; }
        public string TarifaHora { get; set; } = string.Empty;
        public string EnlaceGitHub { get; set; } = string.Empty;
        public string EnlaceLinkedIn { get; set; } = string.Empty;
    }
}
