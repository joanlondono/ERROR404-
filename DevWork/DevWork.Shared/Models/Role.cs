using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DevWork.Shared.Models
{
    public class Role : IdentityRole<int>
    {
        // 'Id' (int) y 'Name' (string) ya vienen heredados automáticamente de IdentityRole<int>.
    }
}
