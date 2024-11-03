using System.ComponentModel.DataAnnotations;
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
        /// Atributo de tipo string que representa el nombre del producto.
        /// Debe tener entre 10 y 64 caracteres.
        /// </summary>
        [StringLength(64, MinimumLength = 10, ErrorMessage = "El nombre debe tener entre 10 y 64 caracteres")]
        public required string Name { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo double que representa el precio del producto.
        /// Debe ser un valor positivo menor a 1000000000.
        /// </summary>
        [Range(0, 100000000, ErrorMessage = "El precio debe ser un número positivo y menor a 1000000000")]
        public required double Price { get; set; }

        /// <summary>
        /// Atributo de tipo int que representa el stock disponible del producto.
        /// Debe ser un número entero no negativo menor a 100000.
        /// </summary>
        [Range(0, 100000, ErrorMessage = "El stock debe ser un número entero no negativo menor a 100,000")]
        public required int Stock { get; set; }

        /// <summary>
        /// Atributo de tipo string que representa la URL de la imagen del producto.
        /// </summary>
        [Required]
        public string ImageUrl { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo ProductType que representa el tipo o categoría del producto.
        /// </summary>
        public ProductType ProductType { get; set; } = null!;
    }
}