using System.ComponentModel.DataAnnotations;

namespace api.src.DTOs
{
    /// <summary>
    /// Clase CreateProductRequestDto que representa la solicitud para crear un nuevo producto.
    /// Contiene las propiedades necesarias para la creación de un producto en la aplicación.
    /// </summary>
    public class CreateProductRequestDto
    {
        /// <summary>
        /// Atributo de tipo string que representa el nombre del producto. 
        /// Debe tener entre 10 y 64 caracteres.
        /// Debe ser una cadena alfabetica.
        /// </summary>
        [Required]
        [StringLength(64, MinimumLength = 10, ErrorMessage = "El nombre debe tener entre 10 y 64 caracteres")]
        [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "El nombre debe consistir solo en letras y espacios")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo int que represente el precio del producto. Debe ser un valor positivo menor a 1000000000.
        /// </summary>
        [Required]
        [Range(0, 100000000, ErrorMessage = "El precio debe ser un número entero positivo y menor a 1000000000")]
        public int Price { get; set; }

        /// <summary>
        /// Atributo de tipo int que representa el stock disponible del producto. Debe ser un número entero no negativo menor a 100000.
        /// </summary>
        [Required]
        [Range(0, 100000, ErrorMessage = "El stock debe ser un número entero no negativo menor a 100,000")]
        public int Stock { get; set; }

        /// <summary>
        /// Atributo de tipo IFormFile que representa la Imagen del producto.
        /// </summary>
        [Required]
        public IFormFile Image { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo int que representa el ID del tipo de producto, que especifica la categoría a la que pertenece el producto.
        /// </summary>
        [Required]
        public int ProductTypeId { get; set; } 
    }
}