using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.src.DTOs.User
{
    public class UpdatePasswordRequestDto
    {
        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]+$", ErrorMessage = "Password must be alphanumeric and contain at least one letter and one number.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 20 and 8 characters")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]+$", ErrorMessage = "Password must be alphanumeric and contain at least one letter and one number.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 20 and 8 characters")]
        public string newPassword { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]+$", ErrorMessage = "Password must be alphanumeric and contain at least one letter and one number.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 20 and 8 characters")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}