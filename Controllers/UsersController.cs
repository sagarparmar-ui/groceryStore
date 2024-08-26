using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RMS.Services.CurrentUserService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RMS.Services.UserService;
using RMS.ViewModels;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RMS.Models;

namespace RMS.Controllers
{
    //[AllowAnonymous]
    public class UsersController : Controller
    {
        private readonly ICurrentUserService _loggedInUser;
        private readonly IUserService _usersInDB;

        public UsersController(IUserService users, ICurrentUserService currentUser)
        {
            _loggedInUser = currentUser;
            _usersInDB = users;
        }

        //Show user list
        [Authorize(Roles = "Admin")]
        public IActionResult UserList()
        {
            //Get User List from Database
            var _userList = _usersInDB.GetAll();

            //Image of the user
            foreach (var _user in _userList)
            {
                _user.ProfilePic = $"data:image/png;base64,{_user.ProfilePic}";
            }

            var _totalUsers = _usersInDB.GetAll().Count();
            TempData["totalUsers"] = _totalUsers;

            return View(_userList);
        }

        //User/Login
        public IActionResult Login()
        {
            return View();
        }

        //User/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginVM model)
        {

            var _user = new User
            {
                Email = model.Email,
                Password = model.Password,
                RememberMe = model.RememberMe
            };

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid Model State";
                return View(model);
            }

            if (IsAdmin(_user.Email, _user.Password))
            {

            }
            else if (!UserExists(_user.Email, _user.Password))
            {
                TempData["Error"] = "You have not registered yet go to Register page and register first.";
                return View(model);
            }

            var user = _usersInDB.GetByEmail(_user.Email);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, _user.Email),
                new Claim(ClaimTypes.Name, $"{user?.FName ?? string.Empty} {user?.LName ?? string.Empty}"),
                new Claim(ClaimTypes.Role, user?.UserType ?? string.Empty)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = _user.RememberMe
            };

            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (user != null && user.ProfilePic != null)
            {
                //Render image from base64
                user.ProfilePic = $"data:image/png;base64,{user.ProfilePic}";
                HttpContext.Session.SetString("ProfilePic", user.ProfilePic);
            }

            // Store the user ID in the session
            byte[] userIdBytes = Encoding.ASCII.GetBytes(user?.Id.ToString() ?? string.Empty);
            HttpContext.Session.Set("UserId", userIdBytes);

            TempData["success"] = "Logged in successfully";
            return RedirectToAction("Index", "Home");

        }

        //User/Register
        public IActionResult Register()
        {
            return View();
        }

        //User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterVM model)
        {
            //Custom Validations
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid Model State";
                return View(model);
            }

            if (model.TermsAndConditions == false)
            {
                ModelState.AddModelError("TermsAndConditions", "You must agree to the terms and conditions.");
                return View(model);
            }

            if (model.UserType == "Admin")
            {
                ModelState.AddModelError("UserType", "You can not register as admin.");
                return View(model);
            }

            var userMail = _usersInDB.GetByEmail(model.Email);
            if (userMail != null)
            {
                TempData["error"] = "You are already registered Go to login page.";
                return View(model);
            }

            var _user = new User
            {
                FName = model.FName,
                LName = model.LName,
                Email = model.Email,
                Password = model.Password,
                DOB = model.DOB,
                Gender = model.Gender,
                UserType = model.UserType,
                Phoneno = model.Phoneno,
                Address = model.Address,
                CreatedOn = DateTime.UtcNow
            };

            //Convert Image From IFormFile to base64
            if (model.ProfilePic?.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    model.ProfilePic.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    string base64image = Convert.ToBase64String(fileBytes);
                    // act on the Base64 data
                    _user.ProfilePic = base64image;
                }
            }

            HashPassword(_user, _user.Password);

            _user.CreatedOn = DateTime.Now;
            _usersInDB.Add(_user);

            _usersInDB.SaveChanges();

            //Show success message
            TempData["success"] = "You have been registered as "+_user.UserType+" successfully";
            return RedirectToAction("Login", "Users");
        }

        //Show AccessDenied to Unauthorised users
        public IActionResult AccessDenied()
        {
            return View();
        }

        //My profile page with user profile details
        public IActionResult MyProfile()
        {
            var _loggedinUser = _loggedInUser.GetLoggedInUser();

            if (_loggedinUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            //Render image from base64
            _loggedinUser.ProfilePic = $"data:image/png;base64,{_loggedinUser.ProfilePic}";

            // Pass the user's information to the view
            return View(_loggedinUser);
        }

        //Update a user
        public IActionResult Update()
        {
            var _loggedinUser = _loggedInUser.GetLoggedInUser();

            if (_loggedinUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            var _returnModel = new UpdateUserVM
            {
                FName = _loggedinUser.FName,
                LName = _loggedinUser.LName,
                Email = _loggedinUser.Email,
                DOB = _loggedinUser.DOB,
                Gender = _loggedinUser.Gender,
                Phoneno = _loggedinUser.Phoneno,
                Address = _loggedinUser.Address
            };

            //Convert base64 to IFormFile.
            if (!string.IsNullOrEmpty(_loggedinUser.ProfilePic))
            {
                var fileBytes = Convert.FromBase64String(_loggedinUser.ProfilePic);
                var formFile = new FormFile(new MemoryStream(fileBytes), 0, fileBytes.Length, "ProductImage", "product.jpg");
                _returnModel.ProfilePic = formFile;
            }

            // Pass the user's information to the view
            return View(_returnModel);
        }

        //Submit updated details
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(UpdateUserVM model)
        {
            // Get the current user's ID from the session
            var _loggedinUser = _loggedInUser.GetLoggedInUser();

            if (_loggedinUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Update the user's information
            _loggedinUser.FName = model.FName;
            _loggedinUser.LName = model.LName;
            _loggedinUser.Email = model.Email;
            _loggedinUser.DOB = model.DOB;
            _loggedinUser.Gender = model.Gender;
            _loggedinUser.Phoneno = model.Phoneno;
            _loggedinUser.Address = model.Address;

            _loggedinUser.UpdatedOn = DateTime.Now;
            _loggedinUser.UpdatedBy = _loggedinUser.Id;

            //Convert Image From IFormFile to base64
            if (model.ProfilePic?.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    model.ProfilePic.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    string base64image = Convert.ToBase64String(fileBytes);
                    // act on the Base64 data
                    _loggedinUser.ProfilePic = base64image;
                }
            }

            // Save the changes to the database
            try
            {
                _usersInDB.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(_loggedinUser.Email, _loggedinUser.Password))
                {
                    TempData["Error"] = "User not found.";
                    return RedirectToAction(nameof(Index), "Home");
                }
                else
                {
                    throw;
                }
            }
            TempData["success"] = "Profile updated successfully";

            return RedirectToAction(nameof(MyProfile));
        }

        //Logout User
        public IActionResult LogOutConfirm()
        {
            // Clear authentication cookies and claims
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear session data
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Users");
        }

        //GET: Users/GetProfilePicture
        [HttpGet]
        public IActionResult GetProfilePicture()
        {
            // Retrieve the profile picture from the session
            var profilePic = HttpContext.Session.GetString("ProfilePic");

            return Json(profilePic);
        }

        //Check if user exists in database
        private bool UserExists(string email, string password)
        {
            var _user = _usersInDB.GetByEmail(email);

            if (_user != null)
            {
                var user = new User();
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, _user.Password, password);

                return result == PasswordVerificationResult.Success;
            }

            return false;
        }

        //Check if user is admin or not
        private bool IsAdmin(string email, string password)
        {
            var _user = _usersInDB.GetByEmail(email);

            if (_user != null && _user.UserType == "Admin")
            {
                return true;
            }
            return false;
        }

        //Encrypt the user password
        private void HashPassword(User user, string password)
        {
            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, password);
        }
        
        //User Settings
        public IActionResult Delete(string? id)
        {
            // Get the current user's ID from the session
            var _loggedinUser = _loggedInUser.GetLoggedInUser();

            if (_loggedinUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            _loggedinUser.DeletedOn = DateTime.Now;
            _loggedinUser.DeletedBy = _loggedinUser.Id;

            _usersInDB.Delete(_loggedinUser.Id);
            _usersInDB.SaveChanges();

            //Show success message
            TempData["success"] = "Account deleted successfully";
            return RedirectToAction("Login", "Users");
        }

        //Display user Details
        public IActionResult Profile()
        {
            return View();
        }

    }
}
