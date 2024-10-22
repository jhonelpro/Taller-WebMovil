using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace api.src.Models.User
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string? Rut { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "Name must be between 8 and 255 characters.")]
        public string? Name { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [RegularExpression(@"Femenino|Masculino|Prefiero no decirlo|Otro", ErrorMessage = "Gender must be one of the specified values.")]
        public string? Gender { get; set; } = string.Empty;
        [Required]
        public int IsActive { get; set; }
    }
}