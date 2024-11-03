using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.src.Models;
using api.src.Repositories;
using Newtonsoft.Json;

namespace api.src.Service
{
    public class CookieService : ICookieService
    {
         private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void ClearCartItemsInCookie()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                context.Response.Cookies.Delete("ShoppingCart");
            }
        }

        public List<ShoppingCartItem> GetCartItemsFromCookies()
        {
            var cartItems = new List<ShoppingCartItem>();
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                var cartCookie = context.Request.Cookies["ShoppingCart"];
                if (!string.IsNullOrEmpty(cartCookie))
                {
                    cartItems = JsonConvert.DeserializeObject<List<ShoppingCartItem>>(cartCookie);
                }
            }
            return cartItems ?? new List<ShoppingCartItem>();
        }

        public void SaveCartItemsToCookies(List<ShoppingCartItem> cartItems)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                var options = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                };
                context.Response.Cookies.Append("ShoppingCart", JsonConvert.SerializeObject(cartItems), options);
            }
        }
    }
}