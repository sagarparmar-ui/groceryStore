using RMS.Models;

namespace RMS.Services.UserService
{
    public interface IUserService
    {
        IEnumerable<User> GetAll();
        User? GetById(Guid Id);
        User? GetByEmail(string email);
        void Add(User user);
        void Update(User user);
        void Delete(Guid Id);
        void SaveChanges();
    }
}
