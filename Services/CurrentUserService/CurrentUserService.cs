using RMS.Models;
using RMS.Services.UserService;
using System.Text;

namespace RMS.Services.CurrentUserService
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _usersInDB;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, IUserService users)
        {
            _httpContextAccessor = httpContextAccessor;
            _usersInDB = users;
        }

        public User? GetLoggedInUser()
        {
            if(_httpContextAccessor.HttpContext != null)
            {
                // Get the current user's ID from the session
                byte[]? userIdBytes;
                if (!_httpContextAccessor.HttpContext.Session.TryGetValue("UserId", out userIdBytes) || userIdBytes == null)
                {
                    return null; // or throw an exception, depending on your requirements
                }

                Guid userId = new Guid(Encoding.ASCII.GetString(userIdBytes));

                User? user = _usersInDB.GetById(userId);

                return user;
            }
            return null;
        }
    }

}
