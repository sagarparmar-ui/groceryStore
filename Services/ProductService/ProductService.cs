using RMS.Models;
using RMS.GenericRepository;

namespace RMS.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productsInDB;
        private readonly IGenericRepository<ProductCategories> _selectedFac;
        private readonly IGenericRepository<Service> _services;

        public ProductService(IGenericRepository<Product> products, IGenericRepository<ProductCategories> selectedFac, IGenericRepository<Service> services)
        {
            _productsInDB = products;
            _selectedFac = selectedFac;
            _services = services;
        }

        public IEnumerable<Product> GetAll()
        {
            return _productsInDB.GetAll();
        }

        public IEnumerable<Service> GetAllServices()
        {
            return _services.GetAll();
        }

        public Product? GetById(Guid Id)
        {
            return _productsInDB.GetById(Id);
        }

        public void Add(Product product)
        {
            _productsInDB.Add(product);
        }

        public void Update(Product product)
        {
            _productsInDB.Update(product);
        }

        public void Delete(Guid Id)
        {
            _productsInDB.Delete(Id);
        }

        public List<ProductCategories> GetSelectedServicesById(Guid Id)
        {
            var productServices = _selectedFac.GetAll()
                .Where(sf => sf.ProductId == Id)
                .ToList();
            return productServices;
        }

        public void AddAll(IEnumerable<ProductCategories> selectedServices)
        {
            _selectedFac.AddAll(selectedServices);
        }
        public void RemoveAll(IEnumerable<ProductCategories> selectedServices)
        {
            _selectedFac.RemoveAll(selectedServices);
        }
        public void SaveChanges()
        {
            _productsInDB.SaveChanges();
        }
    }
}
