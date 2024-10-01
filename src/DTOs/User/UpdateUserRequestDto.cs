using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.src.DTOs
{
    public class UpdateUserRequestDto
    {
        [StringLength(64, MinimumLength = 10, ErrorMessage = "Name must be between 10 and 64 characters")]
        public string Name { get; set; } = string.Empty;
        public DateTime Birth_Date { get; set; }

        [RegularExpression(@"^(Femenino|Masculino|Prefiero no decirlo|Otro)$", ErrorMessage = "Gender must be one of the following: Femenino, Masculino, Prefiero no decirlo, Otro.")]
        public string Gender { get; set; } = string.Empty;
    }
}