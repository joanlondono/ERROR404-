using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DevWork.Shared.DTOs
{
    public class UpdatePhotoDTO
    {
        [Required(ErrorMessage = "La imagen es obligatoria")]
        public string Photo { get; set; } = string.Empty;
    }
}