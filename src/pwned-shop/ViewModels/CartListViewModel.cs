using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pwned_shop.Models;
using pwned_shop.Data;

namespace pwned_shop.ViewModels
{
    public class CartListViewModel
    {
        public List<Cart> List { get; set; }

        public CartListViewModel()
        {
            List = new List<Cart>();
        }

        public int CartCount
        {
            get
            {
                return List.Sum(c => c.Qty);
            }
        }


        public int AddToCart(Cart c)
        {
            Cart cart = List.FirstOrDefault(ca => ca.ProductId == c.ProductId);

            if (cart != null)
            {
                cart.Qty += c.Qty;
                return c.Qty;
            }

            List.Add(c);

            return c.Qty;
        }

        public int UpdateCart(Cart c)
        {
            Cart cart = List.FirstOrDefault(ca => ca.ProductId == c.ProductId);

            if (cart != null)
            {
                cart.Qty = c.Qty;
                return c.Qty;
            }

            return 0;
        }

        public int RemoveFromCart(Cart c)
        {
            Cart cart = List.FirstOrDefault(ca => ca.ProductId == c.ProductId);

            if (cart != null)
            {
                List.Remove(cart);
                return cart.Qty;
            }

            return 0;
        }
    }
}
