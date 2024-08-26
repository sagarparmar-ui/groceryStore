using RMS.Models;
using RMS.GenericRepository;

namespace RMS.Services.ShoppingCartService
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IGenericRepository<ShoppingCart> _repository;

        public ShoppingCartService(IGenericRepository<ShoppingCart> repository)
        {
            _repository = repository;
        }

        public IEnumerable<ShoppingCart> GetAll()
        {
            return _repository.GetAll();
        }

        public ShoppingCart? GetById(Guid Id)
        {
            return _repository.GetById(Id);
        }

        public List<ShoppingCart> GetAllByProductId(Guid Id)
        {
            var shoppingCarts = _repository.GetAll()
                .Where(b => b.ProductId == Id)
                .ToList();
            return shoppingCarts;
        }

        public void Add(ShoppingCart shoppingCart)
        {
            _repository.Add(shoppingCart);
        }

        public void Update(ShoppingCart shoppingCart)
        {
            _repository.Update(shoppingCart);
        }

        public void Delete(Guid Id)
        {
            _repository.Delete(Id);
        }

        public void RemoveAll(IEnumerable<ShoppingCart> shoppingCarts)
        {
            _repository.RemoveAll(shoppingCarts);
        }

        public void SaveChanges()
        {
            _repository.SaveChanges();
        }
    }
}
