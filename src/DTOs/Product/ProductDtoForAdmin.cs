using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;

namespace api.src.DTOs.Product
{
    /// <summary>
    /// Clase ProductDtoForAdmin que representa los datos de un producto a mostrar al administrador.
    /// Contiene las propiedades necesarias para describir un producto en la aplicación.
    /// </summary>
    public class ProductDtoForAdmin
    {
        public int Id { get; set; }

        public required string Name { get; set; } = string.Empty;

        public required double Price { get; set; }

        public required int Stock { get; set; }
        
        public string ImageUrl { get; set; } = null!;
        
        public ProductType ProductType { get; set; } = null!;
    }
}