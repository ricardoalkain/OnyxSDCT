using Onyx.ProductService.Entities;

namespace Onyx.ProductService.Endpoints.Models
{
    public class ProductInputModel
    {
        public string? Name { get; set; }
        public string? Color { get; set; }
        public decimal Price { get; set; }

        public Product ToDataModel()
        {
            return new Product
            {
                Name = Name,
                Color = Color,
                Price = Price
            };
        }
    }
}
