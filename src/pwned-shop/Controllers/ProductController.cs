using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using pwned_shop.Data;
using System.Diagnostics;

namespace pwned_shop.Controllers
{
    public class ProductController : Controller
    {
        private readonly PwnedShopDb db;
        public ProductController(PwnedShopDb db)
        {
            this.db = db;
        }

        public IActionResult Index()
        {
            ViewData["Products"] = db.Products.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Search(string searchText)
        {
            if (searchText == null)
                return RedirectToAction("Index");

            var inter = (from u in db.Products.AsEnumerable()
                         where u.ProductName.ToLower().Contains(searchText.ToLower())
                         select u).ToList();

            ViewData["Products"] = inter;
            ViewData["Searched"] = searchText;

            return View("Index");
        }

        public IActionResult Discount()
        {
            //Debug.WriteLine("Testing the discount code");
            
            //Extracting the products that are on discount.
            var discount = db.Products.Where(u => u.Discount != 0).ToList();

            //Using boolean to notify view it is in hot deals.
            //bool hotDeals = true;
            ViewData["hotdeals"] = true;
            ViewData["Products"] = discount;
            //Testing what item is on discount
            //foreach (var x in discount)
            //{
            //    Debug.WriteLine(x.ProductName);
            //}

            return View("Index");
        }
    }
}
