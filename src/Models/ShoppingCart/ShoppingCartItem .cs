namespace api.src.Models
{
    /// <summary>
    /// Clase que representa un elemento dentro de un carrito de compras, incluyendo información sobre la cantidad y el producto.
    /// </summary>
    public class ShoppingCartItem 
    {
        /// <summary>
        /// Atributo de tipo int que representa la cantidad de unidades del producto en el carrito de compras.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Atributo de tipo int que representa el identificador del carrito de compras al que pertenece este elemento.
        /// </summary>
        public int CartId { get; set; }

        /// <summary>
        /// Atributo de tipo ShoppingCart que representa el carrito de compras al que está asociado este elemento.
        /// </summary>
        public ShoppingCart shoppingCart { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo int que representa el identificador del producto asociado a este elemento del carrito.
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// Atributo de tipo Product que representa el producto asociado a este elemento del carrito.
        /// </summary>
        public Product Product { get; set; } = null!;
    }
}