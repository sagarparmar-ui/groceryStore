using RMS.Data;
using Microsoft.EntityFrameworkCore;

namespace RMS.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private GroceryStoreContext _dbContext;
        private DbSet<T> _dbSet;
        
        public GenericRepository(GroceryStoreContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }
        public T? GetById(object id)
        {
            return _dbSet.Find(id);
        }
        public void Add(T obj)
        {
            _dbSet.Add(obj);
        }
        public void Update(T obj)
        {
            _dbSet.Attach(obj);
            _dbContext.Entry(obj).State = EntityState.Modified;
        }

        public void Delete(object id)
        {
            T? existing = _dbSet.Find(id);
            if (existing != null)
            {
                _dbSet.Remove(existing);
            }
        }

        public void RemoveAll(IEnumerable<T> objs)
        {
            _dbContext.Set<T>().RemoveRange(objs);
        }

        public void AddAll(IEnumerable<T> objs)
        {
            _dbContext.Set<T>().AddRange(objs);
        }
        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
