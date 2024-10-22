using System.ComponentModel.DataAnnotations;

namespace api.src.DTOs.User
{
    public class RegisterDto
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
        public string? Gender { get; set; } = string.Empty!;

        [Required]
        [EmailAddress]
        public string? Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string? Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; } = string.Empty;
    }
}