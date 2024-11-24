using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;

namespace api.src.Repositories
{
    /// <summary>
    /// Interfaz para la gestión de cookies, proporcionando métodos para obtener, guardar y limpiar los elementos del carrito de compras.
    /// </summary>
    public interface ICookieService
    {
        /// <summary>
        /// Obtiene los elementos del carrito de compras almacenados en las cookies.
        /// </summary>
        /// <returns></returns>
        List<ShoppingCartItem> GetCartItemsFromCookies();

        /// <summary>
        /// Guarda los elementos del carrito de compras en las cookies.
        /// </summary>
        /// <param name="cartItems">Parámetro que representa una lista de productos o carrito de compras</param> <summary>
        void SaveCartItemsToCookies(List<ShoppingCartItem> cartItems);

        /// <summary>
        /// Limpia los elementos del carrito de compras almacenados en las cookies.
        /// </summary>
        void ClearCartItemsInCookie();
    }
}