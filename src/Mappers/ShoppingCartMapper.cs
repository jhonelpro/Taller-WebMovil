using System.Numerics;
using api.src.DTOs.Product;
using api.src.DTOs.ShoppingCart;
using api.src.Models;

namespace api.src.Mappers
{
    public static class ShoppingCartMapper
    {
        public static CartDto toCartDto(this List<ShoppingCartDto> shoppingCartItemsDto)
        {
            double cartTotalPrice = 0;

            foreach (var item in shoppingCartItemsDto)
            {
                cartTotalPrice += item.TotalPrice;
            }

            var cartDto = new CartDto();
            cartDto.CartItems = shoppingCartItemsDto;
            cartDto.Cart_TotalPrice = cartTotalPrice;

            return cartDto;
        }

        public static ShoppingCartDto ToShoppingCartDto(this Product product, ShoppingCartItem shoppingCartItem)
        {
            return new ShoppingCartDto
            {
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