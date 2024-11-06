using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.src.Data;
using api.src.Interfaces;
using api.src.Models.Token;
using api.src.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace api.src.Service
{
    /// <summary>
    /// Servicio para la creación y gestión de tokens JWT.
    /// Este servicio se encarga de generar un token que representa a un usuario autenticado.
    /// </summary>
    public class TokenService : ITokenService
    {
        /// <summary>
        /// Gestor de usuarios para acceder a la información de los usuarios en la base de datos.
        /// </summary>
        private readonly UserManager<AppUser> _userManager;

        /// <summary>
        /// Configuración de la aplicación que contiene los parámetros necesarios para la generación del token.
        /// </summary>
        private readonly IConfiguration _config;

        /// <summary>
        /// Clave de seguridad simétrica utilizada para firmar el token.
        /// </summary>
        private readonly SymmetricSecurityKey _key;

        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Constructor que inicializa el servicio de token con la configuración y el gestor de usuarios.
        /// </summary>
        /// <param name="config">Instancia de la configuración de la aplicación.</param>
        /// <param name="userManager">Instancia del gestor de usuarios.</param>
        public TokenService(IConfiguration config, UserManager<AppUser> userManager, ApplicationDBContext context)
        {
            _context = context;
            _userManager = userManager;
            _config = config;

            // Obtener la clave de firma del token desde la configuración
            var signinkey = _config["JWT:SigningKey"];
            if (string.IsNullOrEmpty(signinkey))
            {
                throw new ArgumentNullException(nameof(signinkey), "Signing key cannot be null or empty.");
            }

            // Crear la clave de seguridad simétrica a partir de la clave de firma
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signinkey));
        }
        
        /// <summary>
        /// Crea un token JWT para el usuario proporcionado.
        /// Este token incluye las reclamaciones necesarias para identificar al usuario y sus roles.
        /// </summary>
        /// <param name="user">El usuario para el cual se generará el token.</param>
        /// <returns>Un string que representa el token JWT generado.</returns>
        public string CreateToken(AppUser user)
        {
            // Crear la lista de reclamaciones que se incluirán en el token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email!), // Reclamación del email del usuario
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName!), // Reclamación del nombre de usuario
                new Claim(ClaimTypes.NameIdentifier, user.Id), // Reclamación del identificador único del usuario
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Reclamación del ID único del token
            };

            // Obtener los roles del usuario y añadirlos como reclamaciones
            var userRoles = _userManager.GetRolesAsync(user);

            foreach (var role in userRoles.Result)
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); // Reclamación del rol del usuario
            }

            // Crear las credenciales de firma utilizando la clave de seguridad y el algoritmo de firma
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Definir el descriptor del token que contiene la información del token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // Identidad de las reclamaciones
                Expires = DateTime.Now.AddDays(1), // Establecer la fecha de expiración del token a 1 día
                SigningCredentials = creds, // Credenciales para firmar el token
                Issuer = _config["JWT:Issuer"], // Establecer el emisor del token desde la configuración
                Audience = _config["JWT:Audience"] // Establecer la audiencia del token desde la configuración
            };

            // Crear el manejador del token y generar el token a partir del descriptor
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Retornar el token JWT como string
            return tokenHandler.WriteToken(token);
        }

        public async Task AddToBlacklistAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            
            // Extrae el "jti" (ID único del token) y la fecha de expiración
            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var expiration = jwtToken.ValidTo;

            // Si el token tiene un "jti", lo almacena en la blacklist
            if (jti != null)
            {
                var blacklistedToken = new BlacklistedToken
                {
                    TokenId = jti,
                    Expiration = expiration
                };

                _context.BlacklistedTokens.Add(blacklistedToken);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var jti = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

            // Verifica si el "jti" existe en la blacklist
            if (jti != null)
            {
                var blacklistedToken = await _context.BlacklistedTokens
                    .FirstOrDefaultAsync(bt => bt.TokenId == jti);

                if (blacklistedToken != null && blacklistedToken.Expiration > DateTime.UtcNow)
                {
                    return true; // El token está en la blacklist y aún no ha expirado
                }
            }

            return false; // El token no está en la blacklist
        }
    }
}