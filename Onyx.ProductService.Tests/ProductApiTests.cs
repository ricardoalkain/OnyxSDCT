using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Onyx.ProductService.Tests;

/// <summary>
/// Integration Tests for the Product API.
/// </summary>
public class ProductApiTests
{
    private const string BASE_URL = "/products";
    private const string MEDIA_TYPE = "application/json";

    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    internal static Product[] MockProducts() => [
        new() { Id = 1, Name = "Shirt", Color = "Red",    Price = 1.9m, CreatedAt = new DateTime(2024, 8, 10) },
        new() { Id = 2, Name = "Pants", Color = "Blue",   Price = 2.8m, CreatedAt = new DateTime(2024, 8, 10) },
        new() { Id = 3, Name = "Belt",  Color = "Green",  Price = 3.7m, CreatedAt = new DateTime(2024, 8, 10) },
        new() { Id = 4, Name = "Shoes", Color = "Yellow", Price = 4.6m, CreatedAt = new DateTime(2024, 8, 10) },
        new() { Id = 5, Name = "Coat",  Color = "Black",  Price = 5.5m, CreatedAt = new DateTime(2024, 8, 10) }
    ];

    internal static T? Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
    }

    [Fact]
    public async Task GetAll_MediaType_ApplicationJson()
    {
        // Arrange
        var client = new ProductApiApp().CreateClient();

        // Act
        var response = await client.GetAsync(BASE_URL);

        // Assert
        response.Content.Headers.ContentType.Should().NotBeNull();
        response.Content.Headers.ContentType?.MediaType.Should().BeEquivalentTo("application/json");
    }

    [Fact]
    public async Task GetAll_HasProducts_List()
    {
        // Arrange
        var client = new ProductApiApp().CreateClient();
        var expected = MockProducts();

        // Act
        var response = await client.GetAsync(BASE_URL);

        // Assert
        response.EnsureSuccessStatusCode();

        var actual = Deserialize<Product[]>(await response.Content.ReadAsStringAsync());
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAll_NoProducts_Empty()
    {
        // Arrange
        var client = new ProductApiApp(false).CreateClient();

        // Act
        var response = await client.GetAsync(BASE_URL);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var actual = Deserialize<Product[]>(await response.Content.ReadAsStringAsync());
        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_FilterByExistingColor_List()
    {
        // Arrange
        var client = new ProductApiApp().CreateClient();
        var expected = MockProducts().Where(p => p.Color == "Red");

        // Act
        var response = await client.GetAsync(BASE_URL + "?color=rEd");

        // Assert
        response.EnsureSuccessStatusCode();

        var actual = Deserialize<Product[]>(await response.Content.ReadAsStringAsync());
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAll_FilterByInvalidColor_Empty()
    {
        // Arrange
        var client = new ProductApiApp().CreateClient();

        // Act
        var response = await client.GetAsync(BASE_URL + "?color=FFEE00");

        // Assert
        response.EnsureSuccessStatusCode();

        var actual = Deserialize<Product[]>(await response.Content.ReadAsStringAsync());
        actual.Should().BeEmpty();
    }



    [Fact]
    public async Task GetById_NonExisting_NotFound()
    {
        // Arrange
        var client = new ProductApiApp().CreateClient();

        // Act
        var response = await client.GetAsync(BASE_URL + "/99999");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_Valid_Created()
    {
        // Arrange
        var client = new ProductApiApp().CreateClient();
        var newProduct = new Product
        {
            Name = "Test Product",
            Color = "White",
            Price = 9.9m
        };

        // Act
        var content = new StringContent(JsonSerializer.Serialize(newProduct), Encoding.UTF8, MEDIA_TYPE);
        var response = await client.PostAsync(BASE_URL, content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var location = response.Headers.Location?.PathAndQuery;
        location.Should().NotBeNullOrEmpty();

        response = await client.GetAsync(location);
        response.EnsureSuccessStatusCode();
        var created = Deserialize<Product>(await response.Content.ReadAsStringAsync());
        created.Should().BeEquivalentTo(newProduct,
                                        options => options.Excluding(p => p.Id)
                                                          .Excluding(p => p.CreatedAt)
                                       );
    }

    [Fact]
    public async Task Update_Existing_Accepted()
    {
        // Arrange
        var client = new ProductApiApp().CreateClient();
        var id = MockProducts().First().Id;
        var newProduct = new Product
        {
            Name = "New Shirt",
            Color = "White",
            Price = 9.9m
        };

        // Act
        var content = new StringContent(JsonSerializer.Serialize(newProduct), Encoding.UTF8, MEDIA_TYPE);
        var response = await client.PutAsync(BASE_URL + "/" + id, content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Accepted);

        var location = response.Headers.Location?.PathAndQuery;
        location.Should().NotBeNullOrEmpty();

        response = await client.GetAsync(location);
        response.EnsureSuccessStatusCode();
        var created = Deserialize<Product>(await response.Content.ReadAsStringAsync());
        created.Should().BeEquivalentTo(newProduct,
                                        options => options.Excluding(p => p.Id)
                                                          .Excluding(p => p.CreatedAt)
                                                          .Excluding(p => p.UpdatedAt)
                                       );
    }

    [Fact]
    public async Task Update_NonExisting_NotFound()
    {
        // Arrange
        var client = new ProductApiApp().CreateClient();
        var newProduct = new Product
        {
            Name = "New Shirt",
            Color = "White",
            Price = 9.9m
        };

        // Act
        var content = new StringContent(JsonSerializer.Serialize(newProduct), Encoding.UTF8, MEDIA_TYPE);
        var response = await client.PutAsync(BASE_URL + "/99999", content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_Existing_NoContent()
    {
        // Arrange
        var client = new ProductApiApp().CreateClient();
        var id = MockProducts().First().Id;

        // Act
        var response = await client.DeleteAsync(BASE_URL + "/" + id);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_NonExisting_NotFound()
    {
        // Arrange
        var client = new ProductApiApp().CreateClient();

        // Act
        var response = await client.DeleteAsync(BASE_URL + "/99999");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}

/// <summary>
/// Product API web application factory.
/// </summary>
internal class ProductApiApp : WebApplicationFactory<Program>
{
    private readonly bool _mockServiceData;

    public ProductApiApp(bool mockServiceData = true)
    {
        _mockServiceData = mockServiceData;
    }

    protected override void ConfigureClient(HttpClient client)
    {
        client.DefaultRequestHeaders.Add("x-api-key", "d48b3f23-c609-4247-a9ee-f315d856664e");
        base.ConfigureClient(client);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        if (_mockServiceData)
        {
            builder.ConfigureServices(services =>
            {
                var mocked = ProductApiTests.MockProducts();

                services.AddSingleton<IProductRepository, ProductRepository>(sp => new ProductRepository(mocked));
            });
        }

        return base.CreateHost(builder);
    }
}
