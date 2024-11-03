namespace api.src.DTOs.Purchase
{
    /// <summary>
    /// Clase PurchaseDto que representa los detalles de una compra, con los datos a mostrar al usuario.
    /// </summary>
    public class PurchaseDto
    {
        /// <summary>
        /// Atributo de tipo int que representa el identificador único de la compra.
        /// </summary>
        public int PurchaseId { get; set; }

        /// <summary>
        /// Atributo de tipo string que representa el nombre de usuario asociado a la compra.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa el correo electrónico del usuario asociado a la compra.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo DateTime que representa la fecha y hora de la transacción de la compra.
        /// </summary>
        public DateTime Transaction_Date { get; set; }

        /// <summary>
        /// Atributo de tipo string que representa el país donde se realizó la compra.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa la ciudad donde se realizó la compra.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa la comuna donde se realizó la compra.
        /// </summary>
        public string Commune { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa la calle de la dirección de la compra.
        /// </summary>
        public string Street { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo double que representa el precio total de la compra.
        /// </summary>
        public double Purchase_TotalPrice { get; set; } = 0;

        /// <summary>
        /// Lista de objetos de tipo SaleItemDto que representa los elementos de venta asociados a la compra.
        /// </summary>
        public List<SaleItemDto> saleItemDtos { get; set; } = new List<SaleItemDto>();
    }
}