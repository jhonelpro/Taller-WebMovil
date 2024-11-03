using System.ComponentModel.DataAnnotations;
using api.src.Models;

namespace api.src.DTOs.Purchase
{
    /// <summary>
    /// Clase CreatePurchaseDto que representa la solicitud para crear una nueva compra.
    /// Contiene las propiedades necesarias para registrar una compra en la aplicación.
    /// </summary>
    public class CreatePurchaseDto
    {
        /// <summary>
        /// Atributo de tipo DateTime que representa la fecha y hora de la transacción.
        /// </summary>
        [Required]
        public DateTime Transaction_Date { get; set; }

        /// <summary>
        /// Atributo de tipo string que representa el país en el que se realizó la compra.
        /// Este campo es requerido.
        /// </summary>
        [Required]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa la ciudad en la que se realizó la compra.
        /// Este campo es requerido.
        /// </summary>
        [Required]
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa la comuna en la que se realizó la compra.
        /// Este campo es requerido.
        /// </summary>
        [Required]
        public string Commune { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa la calle de la dirección de la compra.
        /// Este campo es requerido.
        /// </summary>
        [Required]
        public string Street { get; set; } = string.Empty;

        /// <summary>
        /// Lista de objetos de tipo SaleItem que representa los elementos de venta asociados a esta compra.
        /// Establece una relación con la entidad SaleItem en Entity Framework.
        /// </summary>
        public List<SaleItem> SaleItems { get; } = new();
    }
}