using api.src.Models;

namespace api.src.DTOs.Product
{
    /// <summary>
    /// Clase ShoppingCartDto que representa un artículo dentro de un carrito de compras.
    /// Contiene los datos a mostrar al usuario.
    /// </summary>
    public class ShoppingCartDto
    {
        /// <summary>
        /// Atributo de tipo string que representa el nombre del producto.
        /// </summary>
        public required string Name { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo double que representa el precio del producto.
        /// </summary>
        public required int Price { get; set; }

        /// <summary>
        /// Atributo de tipo string que representa la URL de la imagen del producto.
        /// </summary>
        public string ImageUrl { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo ProductType que representa el tipo de producto al que pertenece el artículo.
        /// </summary>
        public ProductType ProductType { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo int que representa la cantidad del producto en el carrito de compras.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Atributo de tipo double que representa el precio total del artículo en función de la cantidad.
        /// </summary>
        public int TotalPrice { get; set; }
    }
}