using RMS.Models;

namespace RMS.Services.ProductService
{
    public interface IProductService
    {
        IEnumerable<Product> GetAll();
        Product? GetById(Guid Id);
        void Add(Product product);
        void Update(Product product);
        void Delete(Guid Id);
        IEnumerable<Service> GetAllServices();
        List<ProductCategories> GetSelectedServicesById(Guid Id);
        void AddAll(IEnumerable<ProductCategories> selectedServices);
        void RemoveAll(IEnumerable<ProductCategories> selectedServices);
        void SaveChanges();
    }
}
