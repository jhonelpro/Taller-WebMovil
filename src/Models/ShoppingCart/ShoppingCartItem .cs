
namespace api.src.Models
{
    public class ShoppingCartItem 
    {
        public int Quantity { get; set; }

        public int CartId { get; set; }
        public ShoppingCart shoppingCart { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}