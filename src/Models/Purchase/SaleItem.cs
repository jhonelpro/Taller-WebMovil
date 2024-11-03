namespace api.src.Models
{
    /// <summary>
    /// Clase que representa un elemento de venta dentro de una compra, incluyendo información sobre el producto y la cantidad adquirida.
    /// </summary>
    public class SaleItem
    {     
        /// <summary>
        /// Atributo de tipo int que representa la cantidad del producto vendida.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Atributo de tipo double que representa el precio unitario del producto.
        /// </summary>
        public double UnitPrice { get; set; }

        /// <summary>
        /// Atributo de tipo double que representa el precio total por la cantidad vendida del producto.
        /// </summary>
        public double TotalPrice { get; set; }

        /// <summary>
        /// Atributo de tipo int que representa el identificador único de la compra asociada.
        /// </summary>
        public int PurchaseId { get; set; }

        /// <summary>
        /// Atributo de tipo Purchase que representa la compra a la que pertenece este elemento de venta.
        /// </summary>
        public Purchase Purchase { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo int que representa el identificador único del producto vendido.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Atributo de tipo Product que representa el producto asociado a este elemento de venta.
        /// </summary>
        public Product Product { get; set; } = null!;
    }
}