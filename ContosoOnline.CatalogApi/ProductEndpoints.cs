using ContosoOnline.CatalogApi.Data;
using ContosoOnline.CatalogApi.DataModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
namespace ContosoOnline.CatalogApi;

public static class ProductEndpoints
{
    public static void MapProductEndpoints (this IEndpointRouteBuilder routes)
    {
        var productsGroup = routes.MapGroup("/products").WithTags(nameof(Product));

        productsGroup.MapGet("/", async (CatalogDbContext db) =>
        {
            return await db.Products.ToListAsync();
        })
        .WithName("GetAllProducts")
        .WithOpenApi();

        productsGroup.MapGet("/{id}", async Task<Results<Ok<Product>, NotFound>> (Guid id, CatalogDbContext db) =>
        {
            return await db.Products.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Product model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetProductById")
        .WithOpenApi();

        productsGroup.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid id, Product product, CatalogDbContext db) =>
        {
            var affected = await db.Products
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, product.Id)
                    .SetProperty(m => m.Name, product.Name)
                    .SetProperty(m => m.Description, product.Description)
                    .SetProperty(m => m.Price, product.Price)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateProduct")
        .WithOpenApi();

        productsGroup.MapPost("/", async (Product product, CatalogDbContext db) =>
        {
            db.Products.Add(product);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/products/{product.Id}",product);
        })
        .WithName("CreateProduct")
        .WithOpenApi();

        productsGroup.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid id, CatalogDbContext db) =>
        {
            var affected = await db.Products
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteProduct")
        .WithOpenApi();
    }
}
