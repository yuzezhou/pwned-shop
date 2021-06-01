using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using pwned_shop.Utils;
using pwned_shop.BindingModels;
using pwned_shop.Data;
using pwned_shop.Models;
using pwned_shop.ViewModels;

namespace pwned_shop.Controllers
{
    public class CartController : Controller
    {
        private readonly PwnedShopDb db;
        private readonly ILogger<CartController> _logger;

        public CartController(PwnedShopDb db, ILogger<CartController> logger)
        {
            this.db = db;
            _logger = logger;
        }
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                var cartList = HttpContext.Session.GetJson<CartListViewModel>("cart");

                if (cartList == null || cartList.List.Count == 0)
                    return View("EmptyCart");

                foreach (Cart c in cartList.List)
                {
                    c.Product = db.Products.FirstOrDefault(p => p.Id == c.ProductId);
                }

                ViewData["cartList"] = cartList.List;
            }
            else
            {
                string userId = User.FindFirst("userId").Value;
                var user = db.Users.FirstOrDefault(u => u.Id == userId);

                List<Cart> cartList = user.Carts.ToList();

                if (cartList.Count == 0)
                    return View("EmptyCart");

                ViewData["cartList"] = cartList;
            }

            return View();
        }

        [HttpPost]
        public IActionResult UpdateCart([FromBody] CartUpdate cu)
        {
            int cartCount;
            float subTotal;
            float total = 0;

            try
            {
                int productId; int qty;
                productId = cu.ProductId; qty = cu.Qty;
                Debug.WriteLine($"Prod ID:{productId}, Qty: {qty}");
                if (qty <= 0)
                    return Json(new
                    {
                        success = false
                    });

                // if user is not logged in, update cart data in Session State as a Jsonified dict
                if (!User.Identity.IsAuthenticated)
                {
                    var cartList = HttpContext.Session.GetJson<CartListViewModel>("cart");

                    // check if "cart" exists in Session data
                    if (cartList != null)
                    {
                        // update cart item qty if cart item exists, otherwise add new cart item
                        cartList.UpdateCart(new Cart { ProductId = productId, Qty = qty });
                    }
                    // create new cratList Dict if there isn't one in session
                    else
                    {
                        cartList = new CartListViewModel();
                        cartList.UpdateCart(new Cart { ProductId = productId, Qty = qty });
                    }

                    // update "cart" Session data
                    HttpContext.Session.SetJson("cart", cartList);

                    // get latest "cartCount" and set to Session data
                    cartCount = cartList.CartCount;
                    HttpContext.Session.SetInt32("cartCount", cartCount);

                    // for debugging, to delete
                    foreach (Cart c in cartList.List)
                    {
                        Debug.WriteLine($"Prod: {c.ProductId} - {c.Qty}");
                    }
                    Debug.WriteLine("Cart count: " + cartCount);

                    var prod = db.Products.FirstOrDefault(p => p.Id == productId);

                    subTotal = (prod.UnitPrice * qty) * (1 - prod.Discount);

                    foreach (Cart c in cartList.List)
                    {
                        var currProd = db.Products.FirstOrDefault(p => p.Id == c.ProductId);
                        var unitPrice = currProd.UnitPrice;
                        var discount = currProd.Discount;
                        total += unitPrice * c.Qty * (1 - discount);
                    } 
                }
                // else user is logged in, update cart data in SQL db Cart table
                else
                {
                    string userId = User.FindFirst("userId").Value;
                    var cart = db.Carts.FirstOrDefault(c => c.ProductId == productId && c.UserId == userId);

                    // update cart item's qty if exists, otherwise add new Cart object
                    if (cart != null)
                    {
                        cart.Qty = qty;
                    }
                    else
                    {
                        cart = new Cart() { UserId = userId, ProductId = productId, Qty = qty };
                        db.Carts.Add(cart);
                    }
                    db.SaveChanges();

                    // get latest "cartCount" and set to Session data
                    cartCount = db.Users.FirstOrDefault(u => u.Id == userId).Carts.Sum(c => c.Qty);
                    HttpContext.Session.SetInt32("cartCount", cartCount);

                    // for debugging, to delete
                    //foreach (Cart c in db.Users.FirstOrDefault(u => u.Id == userId).Carts)
                    //{
                    //    Debug.WriteLine($"Prod: {c.ProductId} - {c.Qty}");
                    //}
                    //Debug.WriteLine("Cart count: " + cartCount);

                    subTotal = cart.Product.UnitPrice * qty * (1- cart.Product.Discount);

                    foreach (Cart c in db.Users.FirstOrDefault(u => u.Id == userId).Carts)
                    {
                        total += c.Product.UnitPrice * c.Qty * (1 - c.Product.Discount);
                    }
                }

                HttpContext.Session.SetInt32("cartCount", cartCount);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _logger.LogError(ex, $"Error updating cart for {cu}");
                return Json(new
                {
                    success = false
                });
            }

            return Json(new
            {
                success = true,
                cartCount = cartCount,
                subTotal = subTotal.ToString("S$ 0.00"),
                total = total.ToString("S$ 0.00")
            });
        }

        [HttpPost]
        public IActionResult AddToCart(int productId)
        {
            int cartCount;
            try
            {
                // if user is not logged in, update cart data in Session State as
                // a Jsonified CartList object
                if (!User.Identity.IsAuthenticated)
                {
                    var cartList = HttpContext.Session.GetJson<CartListViewModel>("cart");

                    // check if "cart" exists in Session data
                    if (cartList != null)
                    {
                        cartList.AddToCart(new Cart { ProductId = productId, Qty = 1 });
                    }
                    // create new new CartList object if there isn't one in session
                    else
                    {
                        cartList = new CartListViewModel();
                        cartList.AddToCart(new Cart { ProductId = productId, Qty = 1 });
                    }

                    // update "cart" Session data
                    HttpContext.Session.SetJson("cart", cartList);
                    // get latest "cartCount"
                    cartCount = cartList.CartCount;

                    // for debugging, to delete
                    //foreach (Cart c in cartList.List)
                    //{
                    //    Debug.WriteLine($"Prod: {c.ProductId} - {c.Qty}");
                    //}
                    //Debug.WriteLine("Cart count: " + cartCount);
                }
                // else user is logged in, update cart data in SQL db Cart table
                else
                {
                    string userId = User.FindFirst("userId").Value;
                    var cart = db.Carts.FirstOrDefault(c => c.ProductId == productId && c.UserId == userId);

                    // check if cart item for this product exists
                    if (cart != null)
                    {
                        cart.Qty += 1;
                    }
                    // create new Cart object if cart item doesnt exist
                    else
                    {
                        cart = new Cart() { UserId = userId, ProductId = productId, Qty = 1 };
                        db.Carts.Add(cart);
                    }
                    db.SaveChanges();

                    // get latest "cartCount"
                    cartCount = db.Users.FirstOrDefault(u => u.Id == userId).Carts.Sum(c => c.Qty);

                    // for debugging, to delete
                    //foreach (var c in db.Users.FirstOrDefault(u => u.Id == userId).Carts)
                    //{
                    //    Debug.WriteLine($"Prod: {c.ProductId} - {c.Qty}");
                    //}
                    //Debug.WriteLine("Cart count: " + cartCount);
                }

                HttpContext.Session.SetInt32("cartCount", cartCount);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _logger.LogError(ex, $"Error adding to cart for prod Id {productId}");

                return Json(new
                {
                    success = false
                });
            }
            return Json(new { success = true, cartCount = cartCount });
        }

        public IActionResult RemoveFromCart(int productId)
        {
            try
            {
                int cartCount;
                if (!User.Identity.IsAuthenticated)
                {
                    var cartList = HttpContext.Session.GetJson<CartListViewModel>("cart");

                    if (cartList != null)
                    {
                        var removeStatus = cartList.RemoveFromCart(new Cart { ProductId = productId });
                        // for debugging, to delete
                        //Debug.WriteLine("Remove {0}", removeStatus);

                        // update "cart" Session data
                        HttpContext.Session.SetJson("cart", cartList);
                    }
                    // get latest "cartCount" and set to Session data
                    cartCount = cartList.CartCount;
                }
                else
                {
                    string userId = User.FindFirst("userId").Value;
                    var cart = db.Carts.FirstOrDefault(c => c.ProductId == productId && c.UserId == userId);

                    if (cart != null)
                    {
                        db.Carts.Remove(cart);
                        db.SaveChanges();
                    }

                    // get latest "cartCount" and add to Session data
                    cartCount = db.Users.FirstOrDefault(u => u.Id == userId).Carts.Sum(c => c.Qty);
                }

                HttpContext.Session.SetInt32("cartCount", cartCount);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                _logger.LogError(ex, $"Error removing from cart for prod Id {productId}");

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}
