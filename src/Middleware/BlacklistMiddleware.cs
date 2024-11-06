using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Interfaces;
using api.src.Service;

namespace api.src.Middleware
{
    public class BlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public BlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Obtén el servicio ITokenService desde el contenedor de servicios por solicitud
            var tokenService = context.RequestServices.GetService<ITokenService>();

            if (tokenService == null)
            {
                // Manejo en caso de que no se resuelva el servicio
                await _next(context);
                return;
            }

            // Verifica si hay un token en los encabezados de la solicitud
            if (context.Request.Headers.TryGetValue("Authorization", out var token))
            {
                // Extrae el token (asumiendo que viene en el formato "Bearer {token}")
                var tokenValue = token.ToString().Replace("Bearer ", string.Empty);

                // Verifica si el token está en la lista negra
                if (await tokenService.IsTokenBlacklistedAsync(tokenValue))
                {
                    // Si el token está en la lista negra, devuelve un 403 Forbidden
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Token is blacklisted.");
                    return;
                }
            }

            // Llama al siguiente middleware en la cadena
            await _next(context);
        }
    }
}