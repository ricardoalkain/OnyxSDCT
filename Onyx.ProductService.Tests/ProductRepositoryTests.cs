using Onyx.ProductService.Entities;
using Onyx.ProductService.Repositories;

namespace Onyx.ProductService.Tests;

/// <summary>
/// Unit tests for the <see cref="ProductRepository"/> class.
/// </summary>
public class ProductRepositoryTests
{
    public static List<Product> MockData()
    {
        return
        [
            new() { Id = 1, Name = "Shirt", Color = "Red",    Price = 1.9m },
            new() { Id = 2, Name = "Pants", Color = "Blue",   Price = 2.8m },
            new() { Id = 3, Name = "Belt",  Color = "Green",  Price = 3.7m },
            new() { Id = 4, Name = "Shoes", Color = "Yellow", Price = 4.6m },
            new() { Id = 5, Name = "Coat",  Color = "Black",  Price = 5.5m }
        ];
    }

    [Fact]
    public void AddAndGet_ProductFound_Product()
    {
        // Arrange
        var productRepository = new ProductRepository(MockData());
        var product = new Product
        {
            Name = "Test Product",
            Color = "Red",
            Price = 1.2m
        };

        // Act
        var productId = productRepository.Add(product);
        var addedProduct = productRepository.Get(productId);

        // Assert
        Assert.NotNull(addedProduct);
        Assert.Equal(product.Name, addedProduct.Name);
        Assert.Equal(product.Color, addedProduct.Color);
        Assert.Equal(product.Price, addedProduct.Price);
    }

    [Fact]
    public void AddAndGet_ProductNotFound_Null()
    {
        // Arrange
        var productRepository = new ProductRepository(MockData());
        var product = new Product
        {
            Name = "Test Product",
            Color = "Red",
            Price = 1.2m
        };

        // Act
        productRepository.Add(product);
        var notfound = productRepository.Get(-1);

        // Assert
        Assert.Null(notfound);
    }

    [Fact]
    public void Update_ProductFound_True()
    {
        // Arrange
        var productRepository = new ProductRepository(MockData());
        var product = new Product
        {
            Name = "Test Product",
            Color = "Red",
            Price = 1.2m
        };

        var productId = productRepository.Add(product);

        // Act
        var updatedProduct = new Product
        {
            Id = productId,
            Name = "Updated Product",
            Color = "Blue",
            Price = 2.3m
        };

        var isUpdated = productRepository.Update(updatedProduct);

        var fetched = productRepository.Get(productId);

        // Assert
        Assert.True(isUpdated);
        Assert.NotNull(fetched);
        Assert.Equal(updatedProduct.Name, fetched.Name);
        Assert.Equal(updatedProduct.Color, fetched.Color);
        Assert.Equal(updatedProduct.Price, fetched.Price);
    }

    [Fact]
    public void Update_ProductNotFound_False()
    {
        // Arrange
        var productRepository = new ProductRepository(MockData());
        var product = new Product
        {
            Name = "Test Product",
            Color = "Red",
            Price = 1.2m
        };

        var productId = productRepository.Add(product);

        // Act
        var updatedProduct = new Product
        {
            Id = -1,
            Name = "Updated Product",
            Color = "Blue",
            Price = 2.3m
        };

        var isUpdated = productRepository.Update(updatedProduct);

        var fetched = productRepository.Get(productId);

        // Assert
        Assert.False(isUpdated);
        Assert.NotNull(fetched);
        Assert.Equal(product.Name, fetched.Name);
        Assert.Equal(product.Color, fetched.Color);
        Assert.Equal(product.Price, fetched.Price);
    }


    [Fact]
    public void Delete_ProductFound_True()
    {
        // Arrange
        var productRepository = new ProductRepository(MockData());
        var product = new Product
        {
            Name = "Test Product",
            Color = "Red",
            Price = 1.2m
        };

        var productId = productRepository.Add(product);

        // Act
        var isDeleted = productRepository.Delete(productId);
        var deletedProduct = productRepository.Get(productId);

        // Assert
        Assert.True(isDeleted);
        Assert.Null(deletedProduct);
    }

    [Fact]
    public void GetAll_NoProducts_Empty()
    {
        // Arrange
        var productRepository = new ProductRepository([]);
        // Act
        var allProducts = productRepository.GetAll();
        // Assert
        Assert.Empty(allProducts);
    }

    [Fact]
    public void GetAll_ProductsFound_List()
    {
        // Arrange
        var productRepository = new ProductRepository([]);
        List<Product> products = [
            new() {
                Name = "Test Product 1",
                Color = "Red",
                Price = 1.2m
            },
            new() {
                Name = "Test Product 2",
                Color = "Blue",
                Price = 2.3m
            },
            new() {
                Name = "Test Product 3",
                Color = "Green",
                Price = 3.4m
            },
            new() {
                Name = "Test Product 4",
                Color = "Yellow",
                Price = 4.5m
            },
        ];

        foreach (var product in products)
        {
            productRepository.Add(product);
        }

        // Act
        var allProducts = productRepository.GetAll();

        // Assert
        Assert.NotNull(allProducts);
        Assert.NotEmpty(allProducts);
        Assert.Equal(products.Count, allProducts.Count());
        Assert.Collection(allProducts, products.Select(product => new Action<Product>(p =>
        {
            Assert.Equal(product.Name, p.Name);
            Assert.Equal(product.Color, p.Color);
            Assert.Equal(product.Price, p.Price);
        })).ToArray());
    }
}