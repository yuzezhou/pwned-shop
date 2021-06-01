using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using pwned_shop.Utils;
using pwned_shop.Data;
using pwned_shop.Models;
using pwned_shop.ViewModels;
using System.Data.Entity;

namespace pwned_shop.Controllers
{
    public class OrderController : Controller
    {
        private readonly PwnedShopDb db;
        public OrderController(PwnedShopDb db)
        {
            this.db = db;
        }

        [Authorize]
        public IActionResult Index()
        {
            ViewData["UserId"] = User;

            string userId1 = User.FindFirst("userId").Value;
            string userId = userId1;
            User user = db.Users.FirstOrDefault(u => u.Id == userId);
            List<OrderViewModel> ListOfOVM = new List<OrderViewModel>();

            //Add some cart data
            Cart mockCart1 = new Cart();
            mockCart1.UserId = userId;
            mockCart1.ProductId = 2;
            mockCart1.Qty = 3;

            Cart mockCart2 = new Cart();
            mockCart2.UserId = userId;
            mockCart2.ProductId = 4;
            mockCart2.Qty = 2;
           
            //Debug.WriteLine("Test for mock data" + mockCart1.ProductId);
            
            //foreach (var test1 in db.Carts)
            //{
            //    Debug.WriteLine(test1.ProductId);
            //}

            //Extracting the data and including it into a list of objectviewmodel.
            foreach (Order o in user.Orders)
            {
                foreach (OrderDetail od in o.OrderDetails)
                {
                    //Debug.WriteLine($"{o.Timestamp}, {od.ActivationCode}, {od.Product.ImgURL}, {od.Product.ProductName}, {od.Product.ProductDesc}");
                    OrderViewModel temp = new OrderViewModel();
                    temp.ImgURL = od.Product.ImgURL;
                    temp.ProductName = od.Product.ProductName;
                    temp.ProductDesc = od.Product.ProductDesc;
                    temp.Timestamp = o.Timestamp;
                    temp.ActivationCode = od.ActivationCode;
                    

                    ListOfOVM.Add(temp);
                }
            }

            //grouping the list by 
            List<OrderViewModel> ListOfOVM2 = new List<OrderViewModel>();
            var q = ListOfOVM.GroupBy(o => new 
            {
                o.ImgURL ,
                o.ProductName,
                o.ProductDesc,
                o.Timestamp
            });

            ViewData["OVMList"] = ListOfOVM;
            return View();
        }

        public IActionResult Detail(string orderId)
        {
            // TODO: retrieve particular order details
            return Content("Not implemented yet");
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            List<Order> newOrderList = new List<Order>();
            List<OrderDetail> newOrderDetailsList = new List<OrderDetail>();
            int i = 0;

            //generate orderId
            //Add order and orderdetal data into database after purchase.
            var newOrderId = ShortGuid.Shorten(Guid.NewGuid());

            List<Cart> userCart = new List<Cart>();
            string userId = User.FindFirst("userId").Value;

            userCart = db.Users.FirstOrDefault(u => u.Id == userId).Carts.ToList();

            //While we are adding order and orderdetail data into the database, we will populate the view data as well for the reciept
            
            List<CheckOutViewModel> recieptList = new List<CheckOutViewModel>();

            Order newOrder = new Order()
            {
                UserId = userId,
                Id = newOrderId,
                Timestamp = DateTime.Now
            };

            db.Orders.Add(newOrder);
            db.SaveChanges();

            //Debug.WriteLine(newOrder.Id);
            //Debug.WriteLine(newOrder);

            foreach (var cartItem in userCart)
            {
                while (i < cartItem.Qty)
                {
                    //Populate OrderDetail & add to database
                    OrderDetail newOrderDetail = new OrderDetail()
                    {
                        ActivationCode = Guid.NewGuid().ToString(),
                        OrderId = newOrderId,
                        ProductId = cartItem.ProductId
                    };
                    
                    db.OrderDetails.Add(newOrderDetail);
                    db.SaveChanges();

                    //Debug.WriteLine(newOrderDetail);
                    //Debug.WriteLine(newOrderDetail.ActivationCode);

                    //populate the checkoutviewmodel
                    CheckOutViewModel reciept = new CheckOutViewModel()
                    {
                        ImgURL = cartItem.Product.ImgURL,
                        ProductName = cartItem.Product.ProductName,
                        ProductDesc = cartItem.Product.ProductDesc,
                        ActivationCode = newOrderDetail.ActivationCode,
                        Qty = cartItem.Qty,
                        UnitPrice = cartItem.Product.UnitPrice,
                        Discount = cartItem.Product.Discount
                        
                    };
                    
                    recieptList.Add(reciept);
                    i++;
                }
                i = 0;
            }

            var receiptView = recieptList.GroupBy(o => o.ProductName);

            //mapping the orderviewmodel into to the view using viewdata
            ViewData["RecieptView"] = receiptView;

            // create Receipt model and pass it to EmailReceipt.SendReceipt
            var receipt = new Receipt
            {
                OrderId = newOrderId
            };

            foreach (var group in receiptView)
            {
                List<string> activationCodes = group.Select(g => g.ActivationCode).ToList();

                receipt.ReceiptItems.Add(new ReceiptItem
                {
                    ProductName = group.First().ProductName,
                    ActivationCodes = activationCodes,
                    UnitPrice = group.First().UnitPrice,
                    Qty = group.First().Qty,
                    Discount = group.First().Discount
                });
            }

            var emailStatus = await EmailReceipt.SendReceipt(db.Users.FirstOrDefault(u => u.Id == userId).Email, receipt);
            if (!emailStatus.IsSuccessful)
            {
                //Debug.WriteLine("Email receipt unsuccessful");
            }

            //Clearing the Cart table in database after purchase
           foreach (var cartDelete in userCart)
            {
                db.Carts.Remove(cartDelete);
            }
            db.SaveChanges();

            //Remove cart session data
            HttpContext.Session.Remove("cart");
            HttpContext.Session.Remove("cartCount");

            return View();
        }
    } 
}

