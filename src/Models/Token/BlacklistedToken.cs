using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.src.Models.Token
{
    public class BlacklistedToken
    {
        /// <summary>
        /// Identificador único de la BlackList de tokens.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador que representa el jti del token.
        /// </summary>
        public string TokenId { get; set; } = string.Empty; // Almacena el "jti" (JWT ID) del token

        /// <summary>
        /// Fecha de expiración del token.
        /// </summary>
        public DateTime Expiration { get; set; } // Fecha de expiración del token
    }
}