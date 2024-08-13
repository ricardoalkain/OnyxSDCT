using Onyx.ProductService.Entities;

namespace Onyx.ProductService.Repositories.Base
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        T? Get(int id);
        IEnumerable<T> GetAll();
        int Add(T entity);
        bool Update(T entity);
        bool Delete(int id);
    }
}