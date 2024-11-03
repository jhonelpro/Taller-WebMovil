namespace api.src.Models
{
    /// <summary>
    /// Clase que representa un producto en el sistema.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Atributo de tipo int que representa el identificador único del producto.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Atributo de tipo string que representa el nombre del producto.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo double que representa el precio del producto. Es requerido.
        /// </summary>
        public required double Price { get; set; }

        /// <summary>
        /// Atributo de tipo int que representa la cantidad de stock disponible del producto. Es requerido.
        /// </summary>
        public required int Stock { get; set; }

        /// <summary>
        /// Atributo de tipo string que representa la URL de la imagen del producto.
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Atributo de tipo int que representa el identificador del tipo de producto asociado.
        /// </summary>
        public int ProductTypeId { get; set; }  // Cambiado a ProductTypeId
        
        /// <summary>
        /// Atributo de tipo ProductType que representa el tipo de producto asociado.
        /// </summary>
        public ProductType ProductType { get; set; } = null!;

        /// <summary>
        /// Colección de elementos de carrito de compras asociados al producto.
        /// </summary>
        public List<ShoppingCartItem> shoppingCartItems { get; } = new List<ShoppingCartItem>();
        
        /// <summary>
        /// Colección de elementos de venta asociados al producto.
        /// </summary>
        public List<SaleItem> SaleItems { get; } = new List<SaleItem>();
    }
}