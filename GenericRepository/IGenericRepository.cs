using RMS.Models;

namespace RMS.GenericRepository
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T? GetById(object id);
        void Add(T obj);
        void Update(T obj);
        void Delete(object id);
        void RemoveAll(IEnumerable<T> objs);
        void AddAll(IEnumerable<T> objs);
        void SaveChanges();
    }
}
