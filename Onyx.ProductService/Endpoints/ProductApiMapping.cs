using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Onyx.ProductService.Endpoints.Authentication;
using Onyx.ProductService.Endpoints.Models;
using Onyx.ProductService.Entities;
using Onyx.ProductService.Repositories;

namespace Onyx.ProductService.Endpoints;

internal static class ProductApiMapping
{
    public static void RegisterProductEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/products")
                           .WithTags("Product")
                           .AddEndpointFilter<ApiKeyAuthenticationFilter>();

        group.MapGet(   "/{id}", ProductApi.GetById).WithName(nameof(ProductApi.GetById));
        group.MapGet(   "",      ProductApi.GetAll);
        group.MapPost(  "",      ProductApi.Create);
        group.MapPut(   "/{id}", ProductApi.Update);
        group.MapDelete("/{id}", ProductApi.Delete);
    }
}

internal class ProductApi
{
    /// <summary>
    /// Lists all products.
    /// </summary>
    /// <param name="name">Optional. Name of the products to search for.</param>
    /// <param name="color">Optional. Color of the products to search for.</param>
    /// <returns>List of registered products.</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product[]))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public static IResult GetAll(IProductRepository repository, string? name = null, string? color = null)
    {
        var fetched = (string.IsNullOrWhiteSpace(color) && string.IsNullOrWhiteSpace(name))
            ? repository.GetAll()
            : repository.GetBy(name, color);

        return Results.Ok(fetched);
    }

    /// <summary>
    /// Fetches a product by its ID.
    /// </summary>
    /// <param name="id">ID of the product.</param>
    /// <returns>Product found.</returns>
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Product[]))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public static IResult GetById(IProductRepository repository, int id)
    {
        var product = repository.Get(id);
        return product is not null
            ? Results.Ok(product)
            : Results.NotFound();
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="product">Payload containing the product data.</param>
    /// <returns>Link to the created product.</returns>
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Product[]))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorOutputModel))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public static IResult Create(IProductRepository repository, IValidator<Product> validator, ProductInputModel product)
    {
        var dto = product.ToDataModel();

        var errors = Validate(dto, validator);
        if (errors is not null)
        {
            return Results.BadRequest(errors);
        }

        var id = repository.Add(dto);
        return Results.CreatedAtRoute(nameof(ProductApi.GetById), new { id });
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">ID of the product to be updated</param>
    /// <param name="product">Payload containing the new product data.</param>
    /// <returns></returns>
    [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(Product[]))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorOutputModel))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public static IResult Update(IProductRepository repository, IValidator<Product> validator, int id, ProductInputModel product)
    {
        var dto = product.ToDataModel();

        var errors = Validate(dto, validator);
        if (errors is not null)
        {
            return Results.BadRequest(errors);
        }

        dto.Id = id;
        return repository.Update(dto)
            ? Results.AcceptedAtRoute(nameof(ProductApi.GetById), new { id })
            : Results.NotFound();
    }

    /// <summary>
    /// Removes a product.
    /// </summary>
    /// <param name="id">Id of the product to be removed.</param>
    [ProducesResponseType(StatusCodes.Status204NoContent, Type = typeof(Product[]))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public static IResult Delete(IProductRepository repository, int id)
    {
        return repository.Delete(id)
            ? Results.NoContent()
            : Results.NotFound();
    }

    private static ErrorOutputModel? Validate<T>(T entity, IValidator<T> validator)
    {
        var result = validator.Validate(entity);
        if (!result.IsValid)
        {
            return new ErrorOutputModel
            {
                Errors = result.Errors.Select(e => e.ErrorMessage).ToArray()
            };
        }

        return null;
    }
}
