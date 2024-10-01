using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace api.src.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [RegularExpression(@"^\d{7,8}-[0-9Kk]$", ErrorMessage = "Invalid RUT format. The format should be 12345678-9 or 12345678-K.")]
        public string Rut { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-ZÀ-ÿ\s]+$", ErrorMessage = "Name must only contain letters and spaces.")]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "Name must be between 255 and 8 characters")]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime Birth_Date { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [RegularExpression(@"^(Femenino|Masculino|Prefiero no decirlo|Otro)$", ErrorMessage = "Gender must be one of the following: Femenino, Masculino, Prefiero no decirlo, Otro.")]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]+$", ErrorMessage = "Password must be alphanumeric and contain at least one letter and one number.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Password must be between 20 and 8 characters")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Range(0, 1, ErrorMessage = "IsActive must be 0 or 1.")]
        public int IsActive { get; set; }

        //EtityFramework relationship
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}