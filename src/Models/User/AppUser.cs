using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace api.src.Models.User
{
    public class AppUser : IdentityUser
    {
        public string? Rut { get; set; } = string.Empty;

        public string? Name { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string? Gender { get; set; } = string.Empty;

        public int IsActive { get; set; }
    }
}