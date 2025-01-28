using ContosoOnline.OrderApi.Data;
using ContosoOnline.OrderApi.DataModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
namespace ContosoOnline.OrderApi;

public static class CartEndpoints
{
    public static void MapCartEndpoints (this IEndpointRouteBuilder routes)
    {
        // APIs for the group will be in the form of "/carts/*"
        var cartsGroup = routes.MapGroup("carts").WithTags(nameof(Cart));

        // create a new, empty cart post to "/cart/"
        cartsGroup.MapPost("/", async (Cart cart, OrderDbContext db) =>
        {
            db.Cart.Add(cart);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/carts/{cart.Id}", cart);
        })
        .WithName("CreateCart")
        .WithOpenApi();

        // get a cart by id "/cart/{id}"
        cartsGroup.MapGet("/{id}", async Task<Results<Ok<Cart>, NotFound>> (Guid id, OrderDbContext db) =>
        {
            return await db.Cart.AsNoTracking()
                .FirstOrDefaultAsync(model => model.Id == id)
                is Cart model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetCartById")
        .WithOpenApi();

        var cartItemsGroup = cartsGroup.MapGroup("{cartId}/items").WithTags(nameof(CartItem));

        // get all items in a cart "/cart/{cartid}/items"
        cartItemsGroup.MapGet("/", List<CartItem> (Guid cartId, OrderDbContext db) =>
        {
            var result = db.CartItem.Where(x => x.CartId == cartId);
            return result.Any() ? result.ToList() : new List<CartItem>();
        })
        .WithName("GetCartItems")
        .WithOpenApi();

        // create a new item in a cart "/cart/{cartid}/items"
        cartItemsGroup.MapPost("/", async (Guid cartId, CartItem cartItem, OrderDbContext db) =>
        {
            db.CartItem.Add(cartItem);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/items/{cartItem.Id}", cartItem);
        })
        .WithName("CreateCartItem")
        .WithOpenApi();

        // update an item in a cart "/cart/{cartid}/items/{id}"
        cartItemsGroup.MapPut("/{id}", async Task<Results<Ok, NotFound>> (Guid cartId, Guid id, CartItem cartItem, OrderDbContext db) =>
        {
            var affected = await db.CartItem
                .Where(model => model.Id == id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.Id, cartItem.Id)
                    .SetProperty(m => m.CartId, cartItem.CartId)
                    .SetProperty(m => m.ProductId, cartItem.ProductId)
                    .SetProperty(m => m.Quantity, cartItem.Quantity)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateCartItem")
        .WithOpenApi();

        // delete an item in a cart
        cartItemsGroup.MapDelete("/{id}", async Task<Results<Ok, NotFound>> (Guid cartId, Guid id, OrderDbContext db) =>
        {
            var affected = await db.CartItem
                .Where(model => model.Id == id)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteCartItem")
        .WithOpenApi();
    }
}
