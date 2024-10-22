using System.ComponentModel.DataAnnotations;

namespace api.src.DTOs.User
{
    public class UserDto
    {
        public string UserName { get; set; } = null!;
        
        public string? Rut { get; set; } = string.Empty;

        public string? Name { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string? Gender { get; set; } = string.Empty!;

        public string? Email { get; set; } = string.Empty;

        public int IsActive { get; set; }
    }
}