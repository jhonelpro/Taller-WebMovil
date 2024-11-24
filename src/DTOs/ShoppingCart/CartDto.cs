using api.src.DTOs.Product;

namespace api.src.DTOs.ShoppingCart
{
    /// <summary>
    /// Clase CartDto que representa un carrito de compras, con los datos a mostrar al usuario.
    /// </summary>
    public class CartDto
    {
        /// <summary>
        /// Atributo de tipo double que representa el precio total del carrito de compras.
        /// </summary>
        public int Cart_TotalPrice { get; set; }

        /// <summary>
        /// Atributo de tipo List<ShoppingCartDto> que contiene los artículos en el carrito de compras.
        /// </summary>
        public List<ShoppingCartDto> CartItems { get; set; } = new List<ShoppingCartDto>();
    }
}