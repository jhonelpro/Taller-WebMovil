using api.src.DTOs.Product;
using api.src.Models;

namespace api.src.Mappers
{
    public static class ShoppingCartMapper
    {
        public static ShoppingCartDto ToShoppingCartDto(this Product product, ShoppingCartItem shoppingCartItem)
        {
            return new ShoppingCartDto
            {
                Name = product.Name,
                Price = product.Price,
                ProductType = product.ProductType,
                ImageUrl = product.ImageUrl,
                Quantity = shoppingCartItem.Quantity
            };
        }
    }
}