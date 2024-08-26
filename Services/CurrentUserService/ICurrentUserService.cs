using RMS.Models;

namespace RMS.Services.CurrentUserService
{
    public interface ICurrentUserService
    {
        User? GetLoggedInUser();
    }
}
