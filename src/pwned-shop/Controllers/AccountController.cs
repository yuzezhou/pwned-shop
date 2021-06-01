using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Diagnostics;
using pwned_shop.Utils;
using pwned_shop.BindingModels;
using pwned_shop.ViewModels;
using pwned_shop.Models;
using pwned_shop.Data;
using Microsoft.EntityFrameworkCore;

namespace pwned_shop.Controllers
{
    public class AccountController : Controller
    {
        private readonly PwnedShopDb db;
        private readonly ILogger<AccountController> _logger;

        public AccountController(PwnedShopDb db, ILogger<AccountController> logger)
        {
            this.db = db;
            _logger = logger;
        }


        public IActionResult Login(string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginDetails login, string returnUrl)
        {
            // find user by email
            var user = db.Users.FirstOrDefault(u => u.Email == login.Email);
            if (user != null)
            {
                // check password hash if matched
                string pwdHash = PasswordHasher.Hash(login.Password, user.Salt);
                if (pwdHash == user.PasswordHash)
                {
                    try
                    {
                        // declare claims
                        var claims = new List<Claim>
                    {
                        new Claim("email", user.Email),
                        new Claim("role", "Member"),
                        new Claim("fullName", user.FirstName + " " + user.LastName),
                        new Claim("userId", user.Id.ToString())
                    };

                        // configure authentication
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            AllowRefresh = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20) // authentication ticket expiry
                        };

                        // sign in with new Identity, authentication name: "Cookies", User.Identity.Name is "fullName"
                        await HttpContext.SignInAsync(new ClaimsPrincipal(
                            new ClaimsIdentity(claims, "Cookies", "fullName", "role")),
                                authProperties);

                        // transfer cart data in session into User's cart
                        var cartList = HttpContext.Session.GetJson<CartListViewModel>("cart");

                        // if cart in Session not empty, override db Cart data
                        if (cartList != null)
                        {
                            // remove existing db Cart data
                            foreach (Cart c in user.Carts)
                            {
                                db.Carts.Remove(c);
                            }

                            // populate new Cart data from Session into db
                            foreach (Cart c in cartList.List)
                            {
                                c.UserId = user.Id;
                                db.Carts.Add(c);
                            }

                            db.SaveChanges();
                        }

                        // get cartCount and save in Session
                        int cartCount = user.Carts.Sum(c => c.Qty);
                        HttpContext.Session.SetInt32("cartCount", cartCount);

                        return Redirect(returnUrl == null ? "/" : returnUrl);
                    }
                    catch (Exception ex)
                    {
                        //Debug.WriteLine("Error occured during login: " + ex.Message);
                        _logger.LogError(ex, "Error occured during login");

                        TempData["error"] = "Something went wrong";
                        return RedirectToAction("Login", new { returnUrl = returnUrl });
                    }
                    
                }
                else
                {
                    TempData["error"] = "Invalid password";
                    return RedirectToAction("Login", new { returnUrl = returnUrl });
                }
            }
            else
            {
                TempData["error"] = "Invalid account";
                return RedirectToAction("Login", new { returnUrl = returnUrl });
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Product");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register([FromForm] UserRegDetails user)
        {   
            // If there are no existing users with same email address, create new user object and add to database.
            // Returns success page. Otherwise redirect to home.

            User test = db.Users.FirstOrDefault(x => x.Email==user.Email);
            ValidateRegistration test2 = new ValidateRegistration();
            if (test2.ValidateDOB(user.DOB) && test2.ValidateEmail(user.Email) && test2.ValidatePassword(user.Password))
            {
                if (test == null)
                {
                    var result = PasswordHasher.CreateHash(user.Password);
                    User newUser = new User();
                    newUser.Id = ShortGuid.Shorten(Guid.NewGuid());
                    newUser.FirstName = user.FirstName;
                    newUser.LastName = user.LastName;
                    newUser.Email = user.Email;
                    newUser.PasswordHash = result[0];
                    newUser.Salt = result[1];
                    newUser.DOB = Convert.ToDateTime(user.DOB);
                    newUser.Address = user.Address;
                    db.Users.Add(newUser);
                    db.SaveChanges();
                    return View("Success");

                }
                else
                {
                    ViewData["Error"] = "Error: Account creation failed because email has already been used.";
                    return View("Register");
                }
            }
            else
            {
                ViewData["Error"] = "Error: Incorrect data was given.";
                return View("Register");
            }

            
        }

        public IActionResult Denied()
        {
            return Content("Not implemented yet");
        }
    }
}
