namespace api.src.DTOs.Purchase
{
    /// <summary>
    /// Clase SaleItemDto que representa un artículo de venta dentro de una compra, con los datos a mostrar al usuario.
    /// </summary>
    public class SaleItemDto
    {
        /// <summary>
        /// Atributo de tipo string que representa el nombre del producto.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo string que representa el tipo de producto (categoría) al que pertenece el producto.
        /// </summary>
        public string ProductType { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo int que representa la cantidad del producto vendido.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Atributo de tipo double que representa el precio unitario del producto.
        /// </summary>
        public double UnitPrice { get; set; }

        /// <summary>
        /// Atributo de tipo double que representa el precio total del artículo de venta (cantidad multiplicada por el precio unitario).
        /// </summary>
        public double TotalPrice { get; set; }
    }
}