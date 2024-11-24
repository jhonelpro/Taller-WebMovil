using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Interfaces;
using api.src.Service;

namespace api.src.Middleware
{
    /// <summary>
    /// Middleware que verifica si un token está en la Blacklist.
    /// </summary>
    /// <remarks>
    /// Este es un middleware de creado para verificar si un token está en la lista negra.
    /// </remarks>
    public class BlacklistMiddleware
    {

        /// <summary>
        /// Atributo que representa el siguiente middleware que debería ejecutarse en el sistema.
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// Constructor que inicializa el middleware con el siguiente a ejecutarse en el sistema.
        /// </summary>
        /// <param name="next">Parámetro que representa el siguiente middleware a ejecutarse en el sistema.</param>
        public BlacklistMiddleware(RequestDelegate next)
        {
            /// <summary>
            /// Se inicializa el middleware con el siguiente middleware a ejecutarse en el sistema.
            /// </summary>
            _next = next;
        }  

        /// <summary>
        /// Método que se encarga de verificar si un token está en la lista negra.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            // Obtén el servicio ITokenService desde el contenedor de servicios por solicitud.
            var tokenService = context.RequestServices.GetService<ITokenService>();

            if (tokenService == null)
            {
                // Manejo en caso de que no se resuelva el servicio.
                await _next(context);
                return;
            }

            // Verifica si hay un token en los encabezados de la solicitud.
            if (context.Request.Headers.TryGetValue("Authorization", out var token))
            {
                // Extrae el token.
                var tokenValue = token.ToString().Replace("Bearer ", string.Empty);

                // Verifica si el token está en la lista negra.
                if (await tokenService.IsTokenBlacklistedAsync(tokenValue))
                {
                    // Si el token está en la lista negra, devuelve un código de estado 403 que significa que no está autorizado.
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Token is blacklisted.");
                    return;
                }
            }

            // Llama al siguiente middleware a ejecutarse en el sistema.
            await _next(context);
        }

    }
}