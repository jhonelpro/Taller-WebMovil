using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;

namespace api.src.Repositories
{
    public interface ICookieService
    {
        List<ShoppingCartItem> GetCartItemsFromCookies();
        void SaveCartItemsToCookies(List<ShoppingCartItem> cartItems);
        void ClearCartItemsInCookie();
    }
}