using api.src.Models.User;

namespace api.src.Models
{
    /// <summary>
    /// Clase que representa un carrito de compras, incluyendo información sobre el usuario propietario y los elementos en el carrito.
    /// </summary>
    public class ShoppingCart
    {
        /// <summary>
        /// Atributo de tipo int que representa el identificador único del carrito de compras.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Atributo de tipo DateTime que representa la fecha de creación del carrito de compras.
        /// </summary>
        public DateTime Create_Date { get; set; }

        /// <summary>
        /// Atributo de tipo string que representa el identificador del usuario propietario del carrito de compras.
        /// </summary>
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo AppUser que representa el usuario que posee el carrito de compras.
        /// </summary>
        public AppUser User { get; set; } = null!;

        /// <summary>
        /// Atributo de tipo List<ShoppingCartItem> que contiene los elementos del carrito de compras.
        /// </summary>
        public List<ShoppingCartItem> shoppingCartItems { get; set; } = [];
    }
}