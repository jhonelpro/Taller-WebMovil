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
        public required string Name { get; set; } = string.Empty;

        public required double Price { get; set; }

        public required int Stock { get; set; }

        public string ImageUrl { get; set; } = null!;
        
        public ProductType ProductType { get; set; } = null!;
    }
}