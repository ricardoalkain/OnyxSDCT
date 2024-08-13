using Onyx.ProductService.Entities;

namespace Onyx.ProductService.Repositories;

public interface IProductRepository : Base.IBaseRepository<Product>
{
    IEnumerable<Product> GetBy(string? name, string? color);
}