using Onyx.ProductService.Entities;

namespace Onyx.ProductService.Repositories;

public class ProductRepository : Base.BaseRepository<Product>, IProductRepository
{
    public ProductRepository() : base()
    {
    }

   #region Quick and dirty mockable in-memory storage

    private readonly Dictionary<int, Product>? _dataProvider = null;

    public ProductRepository(IEnumerable<Product> dataProvider) : base()
    {
        _dataProvider = dataProvider.ToDictionary(p => p.Id);
    }

    protected static Dictionary<int, Product> GlobalData { get; } = [];
    protected override Dictionary<int, Product> Data => _dataProvider ?? GlobalData;

    #endregion

    public IEnumerable<Product> GetBy(string? name, string? color)
    {
        return Data.Values.Where(p => name == null || string.Equals(name, p.Name, StringComparison.OrdinalIgnoreCase))
                          .Where(p => color == null || string.Equals(color, p.Color, StringComparison.OrdinalIgnoreCase));
    }
}
