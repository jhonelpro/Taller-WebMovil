using api.src.Models.User;

namespace api.src.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }
        public DateTime Create_Date { get; set; }

        //EtityFramework relationship
        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;

        public List<ShoppingCartItem> shoppingCartItems { get; set; } = [];
    }
}