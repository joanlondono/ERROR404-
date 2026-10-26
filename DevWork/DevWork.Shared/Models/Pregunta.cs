using System;
using System.Collections.Generic;
using System.Text;

namespace DevWork.Shared.Models
{
    internal class Pregunta
    {
        public int Id { get; set; }
        public int PublicacionId { get; set; }
        public string TextoPregunta { get; set; } = string.Empty;
    }
}
