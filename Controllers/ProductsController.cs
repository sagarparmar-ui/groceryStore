using Microsoft.AspNetCore.Authorization;
using RMS.Services.CurrentUserService;
using RMS.Models;
using RMS.Services.ProductService;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Microsoft.AspNetCore.Mvc;
using RMS.ViewModels;
using RMS.Services.ShoppingCartService;
using X.PagedList;

namespace RMS.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ICurrentUserService _loggedInUser;
        private readonly IProductService _productsInDB;
        private readonly IShoppingCartService _shoppingCartItemsInDB;

        public ProductsController(IProductService products, ICurrentUserService currentUser, IShoppingCartService shoppingCart)
        {
            _loggedInUser = currentUser;
            _productsInDB = products;
            _shoppingCartItemsInDB = shoppingCart;
        }

        //Get all products
        public IActionResult ProductList(int? page)
        {
            var _loggedinUser = _loggedInUser.GetLoggedInUser();

            if (_loggedinUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var _listOfProducts = _productsInDB.GetAll();

            if (_loggedinUser.UserType == "Customer")
            {
                _listOfProducts = _productsInDB.GetAll().Where(f => f.AllowShoppingCarts == true);
            }

            //Render image from base64
            foreach (var _hotel in _listOfProducts)
            {
                _hotel.ProductImage = $"data:image/png;base64,{_hotel.ProductImage}";
            }

            int pageSize = 9;
            int pageNumber = (page ?? 1);

            return View(_listOfProducts.ToPagedList(pageNumber, pageSize));
        }

        //Product list for a specific seller
        [Authorize(Roles = "Seller")]
        public IActionResult SellerProductList(int? page)
        {
            var _loggedinUser = _loggedInUser.GetLoggedInUser();

            if (_loggedinUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var _sellerProducts = _productsInDB.GetAll().Where(r => r.SellerId == _loggedinUser.Id);

            //Render image from base64
            foreach (var _product in _sellerProducts)
            {
                _product.ProductImage = $"data:image/png;base64,{_product.ProductImage}";
            }

            int pageSize = 9;
            int pageNumber = (page ?? 1);

            return View(_sellerProducts.ToPagedList(pageNumber, pageSize));
        }

        //Get AddProduct View
        [Authorize(Roles = "Seller , Admin")]
        public IActionResult AddProduct()
        {
            var _returnModel = new ProductDetailsVM();
            _returnModel.Services = new List<ServiceVM>();


            var _categories = _productsInDB.GetAllServices();
            foreach (var _item in _categories)
            {
                var _category = new ServiceVM();
                _category.ServiceId = _item.ServiceId;
                _category.Name = _item.Name;
                _category.IsSelected = _item.IsSelected;
                _returnModel.Services.Add(_category);
            }

            return View(_returnModel);
        }

        [HttpPost]
        [Authorize(Roles = "Seller , Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult AddProduct(ProductDetailsVM model)
        {
            if (!ModelState.IsValid)
            {
                // if model state is invalid, return the view with the invalid model object
                return View(model);
            }

            var _loggedinUser = _loggedInUser.GetLoggedInUser();

            if (_loggedinUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var _product = new Product
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Seller = model.Seller,
                Price = model.Price,
                PriceType = model.PriceType,
                Location = model.Location,
                Email = model.Email,
                Phoneno = model.Phoneno,
                Description = model.Description,
                Services = new List<ProductCategories>(),
                SellerId = _loggedinUser.Id,
            };

            //Convert Image to display in view
            if (model.ProductImage?.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    model.ProductImage.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    string base64image = Convert.ToBase64String(fileBytes);
                    _product.ProductImage = base64image;
                }
            }

            var _categories = model.Services?.Where(f => f.IsSelected == true).ToList();

            if (_categories?.Count > 0)
            {
                foreach (var item in _categories)
                {
                    var _category = new ProductCategories();
                    _category.ServiceId = item.ServiceId;
                    _category.Name = item.Name;
                    _category.IsSelected = item.IsSelected;
                    _product.Services.Add(_category);
                }
            }

            _product.CreatedOn = DateTime.Now;
            _product.CreatedBy = _loggedinUser.Id;

            _productsInDB.Add(_product);
            _productsInDB.SaveChanges();

            TempData["success"] = "New Product added.";
            return RedirectToAction(nameof(ProductList));
        }

        [Authorize(Roles = "Seller , Admin")]
        public IActionResult Delete(string Id)
        {
            //returns json not the page id guid is parameter
            if (Id == null)
            {
                return NotFound();
            }

            Product? _product = _productsInDB.GetById(new Guid(Id));//converted string to guid

            if (_product == null)
            {
                return NotFound();
            }

            _product.Services = new List<ProductCategories>();

            var _resServices = _productsInDB.GetSelectedServicesById(new Guid(Id));
            _product.Services = _resServices;

            _product.ProductImage = $"data:image/png;base64,{_product.ProductImage}";

            return View(_product);
        }

        //Delete a product
        [Authorize(Roles = "Seller , Admin")]
        public IActionResult DeleteConfirm(string Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var _loggedinUser = _loggedInUser.GetLoggedInUser();

            if (_loggedinUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            Product? _product = _productsInDB.GetById(new Guid(Id));//converted string to guid

            if (_product == null) { return NotFound(); }

            var _resFac = _productsInDB.GetSelectedServicesById(_product.Id);

            if (_product == null)
            {
                return NotFound();
            }

            _productsInDB.RemoveAll(_resFac);

            var _productCategories = _shoppingCartItemsInDB.GetAllByProductId(new Guid(Id));
            _shoppingCartItemsInDB.RemoveAll(_productCategories);

            _product.DeletedOn = DateTime.Now;
            _product.DeletedBy = _loggedinUser.Id;

            _productsInDB.Delete(_product.Id);

            _productsInDB.SaveChanges();

            TempData["success"] = "Product deleted successfully";
            return RedirectToAction(nameof(ProductList));
        }

        // GET: Edit Product
        [Authorize(Roles = "Seller , Admin")]
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product? _product = _productsInDB.GetById(new Guid(id));

            if (_product == null)
            {
                return NotFound();
            }

            var _returnModel = new ProductDetailsVM
            {
                Id = _product.Id,
                Name = _product.Name,
                Seller = _product.Seller,
                Price = _product.Price,
                PriceType = _product.PriceType,
                Location = _product.Location,
                Email = _product.Email,
                Phoneno = _product.Phoneno,
                Description = _product.Description,
                Services = new List<ServiceVM>(),
                SellerId = _product.SellerId
            };

            var _resFac = _productsInDB.GetSelectedServicesById(new Guid(id));

            var _categoryIds = _resFac.Select(f => f.ServiceId);

            var _allFac = _productsInDB.GetAllServices()
                .Where(f => !_categoryIds.Contains(f.ServiceId));

            _returnModel.Services.AddRange(_resFac.Select(_item => new ServiceVM
            {
                ServiceId = _item.ServiceId,
                Name = _item.Name,
                IsSelected = _product.Services != null && _product.Services.Any(f => f.ServiceId == _item.ServiceId && f.IsSelected)
            }));

            _returnModel.Services.AddRange(_allFac.Select(_item => new ServiceVM
            {
                ServiceId = _item.ServiceId,
                Name = _item.Name,
                IsSelected = _product.Services != null && _product.Services.Any(f => f.ServiceId == _item.ServiceId && f.IsSelected)
            }));

            //Convert base64 to IFormFile.
            if (!string.IsNullOrEmpty(_product.ProductImage))
            {
                var fileBytes = Convert.FromBase64String(_product.ProductImage);
                var formFile = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, "ProductImage", "product.jpg");
                _returnModel.ProductImage = formFile;
            }

            return View(_returnModel);
        }

        // POST: Edit Product
        [HttpPost, ActionName("Edit")]
        [Authorize(Roles = "Seller , Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult EditConfirmed(string id, ProductDetailsVM model)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // if model state is invalid, return the view with the invalid model object
                return View(model);
            }

            Product? _product = _productsInDB.GetById(new Guid(id));

            if (_product == null)
            {
                return NotFound();
            }

            var _loggedinUser = _loggedInUser.GetLoggedInUser();

            if (_loggedinUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            _product.Name = model.Name;
            _product.Seller = model.Seller;
            _product.Price = model.Price;
            _product.PriceType = model.PriceType;
            _product.Location = model.Location;
            _product.Email = model.Email;
            _product.Phoneno = model.Phoneno;
            _product.Description = model.Description;

            // Convert Image to display in view
            if (model.ProductImage?.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    model.ProductImage.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    string base64image = Convert.ToBase64String(fileBytes);
                    _product.ProductImage = base64image;
                }
            }

            var productServices = _productsInDB.GetSelectedServicesById(_product.Id);
            _productsInDB.RemoveAll(productServices);

            var selectedServices = model.Services?.Where(f => f.IsSelected == true).ToList();

            if (selectedServices?.Count > 0)
            {
                var _selectedfac = selectedServices.Select(item => new ProductCategories
                {
                    ServiceId = item.ServiceId,
                    Name = item.Name,
                    IsSelected = item.IsSelected,
                    ProductId = _product.Id
                }).ToList();

                _productsInDB.AddAll(_selectedfac);
            }

            _product.UpdatedOn = DateTime.Now;
            _product.UpdatedBy = _loggedinUser.Id;

            _productsInDB.SaveChanges();
            TempData["success"] = "Product edited successfully";
            return RedirectToAction(nameof(ProductList));
        }

        //Get Product Details View
        public IActionResult Details(string Id)
        {
            //returns json not the page id guid is parameter
            if (Id == null)
            {
                return NotFound();
            }

            Product? _product = _productsInDB.GetById(new Guid(Id));//converted string to guid

            if (_product == null)
            {
                return NotFound();
            }

            //_product.Services = new List<ProductCategories>();

            _product.Services = _productsInDB.GetSelectedServicesById(new Guid(Id));

            _product.ProductImage = $"data:image/png;base64,{_product.ProductImage}";

            return View(_product);
        }

        //Toggle product visibility for customers.
        [HttpPost]
        public IActionResult ToggleService(Guid productId, bool isChecked)
        {
            var _product = _productsInDB.GetById(productId);
            if (_product == null) { return NotFound(); }
            _product.AllowShoppingCarts = isChecked;

            TempData["success"] = isChecked ? "Service opened for " + _product.Name + " successfully" :
                                              "Service closed for " + _product.Name + " successfully";
            _productsInDB.SaveChanges();

            return Ok(new { success = true });
        }

        //GET: Products/GetProfilePicture
        [HttpGet]
        public IActionResult GetProfilePicture()
        {
            // Retrieve the profile picture from the session
            var profilePic = HttpContext.Session.GetString("ProfilePic");

            return Json(profilePic);
        }
    }
}
