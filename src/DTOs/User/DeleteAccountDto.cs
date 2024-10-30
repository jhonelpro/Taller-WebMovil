using System.ComponentModel.DataAnnotations;

namespace api.src.DTOs.User
{
    public class DeleteAccountDto
    {
        [Required]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 20 characters long.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Password must be alphanumeric.")]
        public string Password { get; set; } = null!;

        public bool ConfirmDeleteAccount { get; set; } = false;
    }
}