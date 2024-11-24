using api.src.Models;

namespace api.src.DTOs
{
    /// <summary>
    /// Clase ProductDto que representa los datos de un producto a mostrar al cliente.
    /// Contiene las propiedades necesarias para describir un producto en la aplicación.
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Atributo que representa el nombre único de un producto.
        /// </summary>
        public required string Name { get; set; } = string.Empty;

        /// <summary>
        /// Atributo que representa el precio de un producto.
        /// </summary>
        public required int Price { get; set; }

        /// <summary>
        /// Atributo que representa la cantidad de stock de un producto.
        /// </summary>
        public required int Stock { get; set; }

        /// <summary>
        /// Atributo que representa el url de la imagen de un producto.
        /// </summary>
        public string ImageUrl { get; set; } = null!;
        
        /// <summary>
        /// Atributo que representa el tipo de un producto.
        /// </summary>
        public ProductType ProductType { get; set; } = null!;
    }
}