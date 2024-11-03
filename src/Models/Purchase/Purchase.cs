using api.src.Models.User;

namespace api.src.Models
{
    /// <summary>
    /// Clase que representa una compra realizada por un usuario en el sistema.
    /// </summary>
    public class Purchase
    {
        /// <summary>
        /// Atributo de tipo int que representa el identificador único de la compra.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Atributo de tipo DateTime que representa la fecha y hora de la transacción.
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
        /// Atributo de tipo string que representa la calle donde se realizó la compra.
        /// </summary>
        public string Street { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa el identificador único del usuario que realizó la compra.
        /// </summary>
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo AppUser que representa el usuario que realizó la compra.
        /// </summary>
        public AppUser User { get; set; } = null!;

        /// <summary>
        /// Lista de elementos de venta asociados a la compra.
        /// </summary>
        public List<SaleItem> SaleItems { get; } = [];
    }
}