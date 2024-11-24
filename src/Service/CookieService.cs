using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;
using api.src.Repositories;
using Newtonsoft.Json;

namespace api.src.Service
{
    /// <summary>
    /// Servicio para la gestión de cookies, proporcionando métodos para obtener, guardar y limpiar los elementos del carrito de compras.
    /// </summary>
    public class CookieService : ICookieService
    {
        /// <summary>
        /// Atributo para acceder al contexto HTTP de la aplicación.
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Constructor que inicializa el servicio de cookies con el contexto HTTP de la aplicación.
        /// </summary>
        /// <param name="httpContextAccessor">Parámetro que representa el contexto HTTP.</param>
        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            // Inicializa el contexto HTTP
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Limpia los elementos del carrito de compras almacenados en las cookies.
        /// </summary>
        public void ClearCartItemsInCookie()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                context.Response.Cookies.Delete("ShoppingCart");
            }
        }

        /// <summary>
        /// Obtiene los elementos del carrito de compras almacenados en las cookies.
        /// </summary>
        /// <returns>Retorna una lista de los productos en el carrito de compras gurada en una cookie.</returns>
        public List<ShoppingCartItem> GetCartItemsFromCookies()
        {
            var cartItems = new List<ShoppingCartItem>();
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                var cartCookie = context.Request.Cookies["ShoppingCart"];
                if (!string.IsNullOrEmpty(cartCookie))
                {
                    cartItems = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cartCookie);
                }
            }
            return cartItems ?? new List<ShoppingCartItem>();
        }

        /// <summary>
        /// Guarda los elementos del carrito de compras en las cookies.
        /// </summary>
        /// <param name="cartItems">Parametro que representa el carrito de compras que se guradara en la cookie.</param>
        public void SaveCartItemsToCookies(List<ShoppingCartItem> cartItems)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                var options = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                };
                context.Response.Cookies.Append("ShoppingCart", JsonConvert.SerializeObject(cartItems), options);
            }
        }
    }
}