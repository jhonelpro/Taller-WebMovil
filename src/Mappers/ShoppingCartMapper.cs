using System.Numerics;
using api.src.DTOs.Product;
using api.src.DTOs.ShoppingCart;
using api.src.Models;

namespace api.src.Mappers
{
    /// <summary>
    /// Clase que proporciona métodos de mapeo entre entidades de carrito de compras y sus DTOs correspondientes.
    /// </summary>
    public static class ShoppingCartMapper
    {
        /// <summary>
        /// Convierte una lista de objetos ShoppingCartDto a un objeto CartDto.
        /// </summary>
        /// <param name="shoppingCartItemsDto">Parametro de tipo List<ShoppingCartDto> que representa los elementos del carrito de compras a convertir.</param>
        /// <returns>Un objeto de tipo CartDto que representa el carrito de compras con sus elementos y el precio total.</returns>
        public static CartDto toCartDto(this List<ShoppingCartDto> shoppingCartItemsDto)
        {
            int cartTotalPrice = 0;

            foreach (var item in shoppingCartItemsDto)
            {
                cartTotalPrice += item.TotalPrice;
            }

            var cartDto = new CartDto();
            cartDto.CartItems = shoppingCartItemsDto;
            cartDto.Cart_TotalPrice = cartTotalPrice;

            return cartDto;
        }

        /// <summary>
        /// Convierte un objeto Product y un objeto ShoppingCartItem a un objeto ShoppingCartDto.
        /// </summary>
        /// <param name="product">Parametro de tipo Product que representa el producto a convertir.</param>
        /// <param name="shoppingCartItem">Parametro de tipo ShoppingCartItem que representa el elemento del carrito de compras asociado al producto.</param>
        /// <returns>Un objeto de tipo ShoppingCartDto que representa el elemento del carrito de compras convertido.</returns>
        public static ShoppingCartDto ToShoppingCartDto(this Product product, ShoppingCartItem shoppingCartItem)
        {
            return new ShoppingCartDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ProductType = product.ProductType,
                ImageUrl = product.ImageUrl,
                Quantity = shoppingCartItem.Quantity,
                TotalPrice = product.Price * shoppingCartItem.Quantity
            };
        }
    }
}