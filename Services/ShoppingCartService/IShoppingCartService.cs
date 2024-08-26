using RMS.Models;

namespace RMS.Services.ShoppingCartService
{
    public interface IShoppingCartService
    {
        IEnumerable<ShoppingCart> GetAll();
        ShoppingCart? GetById(Guid Id);
        List<ShoppingCart> GetAllByProductId(Guid Id);
        void Add(ShoppingCart shoppingCart);
        void Update(ShoppingCart shoppingCart);
        void Delete(Guid Id);
        void RemoveAll(IEnumerable<ShoppingCart> shoppingCarts);
        void SaveChanges();
    }
}
