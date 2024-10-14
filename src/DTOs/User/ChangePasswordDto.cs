using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.src.DTOs.User
{
    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        public string NewPassword { get; set; } = null!;

        [Required]
        public string ConfirmPassword { get; set; } = null!;
    }
}