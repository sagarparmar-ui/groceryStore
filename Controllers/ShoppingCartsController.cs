using RMS.Services.ShoppingCartService;
using Microsoft.AspNetCore.Mvc;
using RMS.Services.UserService;
using RMS.Services.ProductService;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using RMS.Models;
using RMS.Services.CurrentUserService;
using Grocery_Store.ViewModels;

namespace RMS.Controllers
{
    public class ShoppingCartsController : Controller
    {
        private readonly ICurrentUserService _loggedInUser;
        private readonly IProductService _productsInDB;
        private readonly IShoppingCartService _shoppingCartItemsInDBItems;
        private readonly IUserService _usersInDB;

        public ShoppingCartsController(IUserService users, IProductService products, IShoppingCartService shoppingCarts, ICurrentUserService currentUser)
        {
            _loggedInUser = currentUser;
            _productsInDB = products;
            _shoppingCartItemsInDBItems = shoppingCarts;
            _usersInDB = users;
        }

        // GET ShoppingCart items List
        [Authorize(Roles = "Admin")]
        public IActionResult ShoppingCartList()
        {
            var _shoppingCartItemsInDBList = _shoppingCartItemsInDBItems.GetAll();
            return View(_shoppingCartItemsInDBList);
        }

        //All sgopping cart item list for admin
        [Authorize(Roles = "Seller")]
        public IActionResult SellerShoppingCartList()
        {
            var _loggedinUser = _loggedInUser.GetLoggedInUser();

            if (_loggedinUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var _sellerShoppingCartList = _shoppingCartItemsInDBItems.GetAll().Where(b => b.SellerId == _loggedinUser.Id && b.Status == ShoppingCart.StatusEnum.CheckedOut).ToList();
            return View(_sellerShoppingCartList);
        }

        // GET ShoppingCart items List for a Seller
        [Authorize(Roles = "Customer")]
        public IActionResult CustomerShoppingCartList()
        {
            var _loggedinUser = _loggedInUser.GetLoggedInUser();

            if (_loggedinUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var _customerShoppingCartList = _shoppingCartItemsInDBItems.GetAll().Where(b => b.ResidentId == _loggedinUser.Id).ToList();
            return View(_customerShoppingCartList);
        }

        [Authorize(Roles = "Customer")]
        public IActionResult Create(string Id)
        {
            var _product = _productsInDB.GetById(new Guid(Id));
            if (_product == null) { return NotFound(); }

            var _loggedinUser = _loggedInUser.GetLoggedInUser();
            if (_loggedinUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            //if (IsAppointed(Id))
            //{
            //    TempData["error"] = "You have already Added this product";
            //    return RedirectToAction("ProductList", "Products");
            //}

            //Do not allow customer to book the product if seller has closed its shoppingCarts
            if (_product.AllowShoppingCarts == false)
            {
                TempData["error"] = "Deliveries are closed for this product";
                return RedirectToAction("ProductList", "Products");
            }

            var shoppingCart = new ShoppingCart
            {
                Id = Guid.NewGuid(),
                SellerId = _product.SellerId,
                ProductId = _product.Id,
                ResidentId = _loggedinUser.Id,
                ProductName = _product.Name,
                Resident = _loggedinUser.FName + _loggedinUser.LName,
                ResidentPhone = _loggedinUser.Phoneno,
                Price = _product.Price,
                PriceType = _product.PriceType,
                Status = ShoppingCart.StatusEnum.Pending
            };

            shoppingCart.CreatedOn = DateTime.Now;
            shoppingCart.CreatedBy = _loggedinUser.Id;

            _shoppingCartItemsInDBItems.Add(shoppingCart);
            _shoppingCartItemsInDBItems.SaveChanges();

            TempData["success"] = "New Product added.";
            return RedirectToAction(nameof(CustomerShoppingCartList));

        }

        [Authorize(Roles = "Customer , Admin")]
        public IActionResult Checkout(string Id)
        {
            var model = new CheckoutVM();
            model.Id = new Guid(Id);
            
            return View(model);
        }

        //GET: ShoppingCarts/CheckedOut/Id
        [Authorize(Roles = "Customer , Admin")]
        [HttpPost]
        public IActionResult Checkout(CheckoutVM model)
        {
            if (!ItemExists(model.Id.ToString()))
            {
                return NotFound();
            }

            var _loggedinUser = _loggedInUser.GetLoggedInUser();

            if (_loggedinUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var shoppingCart = _shoppingCartItemsInDBItems.GetById(model.Id);

            if (shoppingCart != null)
            {
                if(model.PaymentType == "CashOnDelivery")
                {
                    shoppingCart.PaymentType = model.PaymentType;
                }
                else
                {
                    if (!ModelState.IsValid)
                    {
                        return View(model);
                    }
                    shoppingCart.PaymentType = model.PaymentType;
                    shoppingCart.Name = model.Name;
                    shoppingCart.CardNumber = model.CardNumber;
                    shoppingCart.CVV = model.CVV;
                    shoppingCart.ExpiryDate = model.ExpiryDate;
                }

                shoppingCart.Status = ShoppingCart.StatusEnum.CheckedOut;
                shoppingCart.Date = DateTime.Now;
                TempData["success"] = "Product CheckedOut successfully";
            }
            else
            {
                TempData["error"] = "Unable to find the Product.";
            }

            _shoppingCartItemsInDBItems.SaveChanges();

            if (_loggedinUser.UserType == "Admin")
            {
                return RedirectToAction(nameof(ShoppingCartList));
            }
            else
            {
                return RedirectToAction(nameof(CustomerShoppingCartList));
            }
        }

        //GET: ShoppingCarts/GetProfilePicture
        [HttpGet]
        public IActionResult GetProfilePicture()
        {
            // Retrieve the profile picture from the session
            var profilePic = HttpContext.Session.GetString("ProfilePic");

            return Json(profilePic);
        }

        //GET: ShoppingCarts/Delete/Id
        public IActionResult Delete(string Id)
        {
            if (Id == null || !ItemExists(Id))
            {
                return NotFound();
            }

            var _loggedinUser = _loggedInUser.GetLoggedInUser();

            if (_loggedinUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var shoppingCart = _shoppingCartItemsInDBItems.GetById(new Guid(Id));

            if (shoppingCart != null)
            {
                shoppingCart.DeletedOn = DateTime.Now;
                shoppingCart.DeletedBy = _loggedinUser.Id;

                _shoppingCartItemsInDBItems.Delete(shoppingCart.Id);
                _shoppingCartItemsInDBItems.SaveChanges();
            }

            TempData["success"] = "Product deleted successfully";


            if (_loggedinUser.UserType == "Admin")
            {
                return RedirectToAction(nameof(ShoppingCartList));
            }
            else if (_loggedinUser.UserType == "Seller")
            {
                return RedirectToAction(nameof(SellerShoppingCartList));
            }
            else
            {
                return RedirectToAction(nameof(CustomerShoppingCartList));
            }
        }

        //Check if Item exists in shopping cart
        private bool ItemExists(string Id)
        {
            return (_shoppingCartItemsInDBItems.GetAll()?.Any(e => e.Id == new Guid(Id))).GetValueOrDefault();
        }

    }
}
