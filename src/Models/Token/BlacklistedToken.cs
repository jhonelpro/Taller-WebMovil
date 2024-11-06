using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.src.Models.Token
{
    public class BlacklistedToken
    {
        public int Id { get; set; }
        public string TokenId { get; set; } = string.Empty; // Almacena el "jti" (JWT ID) del token
        public DateTime Expiration { get; set; } // Fecha de expiración del token
    }
}