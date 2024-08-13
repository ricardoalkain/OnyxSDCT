using Onyx.ProductService.Repositories;

namespace Onyx.ProductService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>(service => new ProductRepository());
        return services;
    }
}
