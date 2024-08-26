using Microsoft.AspNetCore.Mvc;
using RMS.Services.UserService;
using RMS.Services.CurrentUserService;

namespace RMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICurrentUserService _loggedInUser;
        private readonly IUserService _usersInDB;

        public HomeController(IUserService users, ICurrentUserService currentUser)
        {
            _loggedInUser = currentUser;
            _usersInDB = users;
        }

        //Load the Dashboard
        public IActionResult Index()
        {
            // Get the current user's ID from the session
            var _loggedinUser = _loggedInUser.GetLoggedInUser();

            if (_loggedinUser == null)
            {
                return RedirectToAction("Login", "Users");
            }

            if (_usersInDB.GetById(_loggedinUser.Id)?.UserType != "Admin")
            {
                return View();
            }

            return View();
        }

        //Dislay Privacy Policy page
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        //Display About us page
        public IActionResult AboutUs()
        {
            return View();
        }

        //GET: Home/GetProfilePicture
        [HttpGet]
        public IActionResult GetProfilePicture()
        {
            // Retrieve the profile picture from the session
            var profilePic = HttpContext.Session.GetString("ProfilePic");

            return Json(profilePic);
        }
    }
}