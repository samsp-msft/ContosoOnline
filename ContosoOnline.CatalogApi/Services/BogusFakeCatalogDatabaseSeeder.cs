using Bogus;
using ContosoOnline.CatalogApi.Data;
using ContosoOnline.CatalogApi.DataModels;
using ContosoOnline.Common;

namespace ContosoOnline.CatalogApi.Services;

public class BogusFakeCatalogDatabaseSeeder : DatabaseSeeder<CatalogDbContext>
{
    public override async Task Seed(CatalogDbContext dbContext)
    {
        if(dbContext is not null && !dbContext.Products.Any())
        {
            var faker = new Faker<Product>()
                            .RuleFor(x => x.Name, f => f.Commerce.ProductName())
                            .RuleFor(x => x.Description, f => f.Commerce.ProductDescription())
                            .RuleFor(x => x.Price, f => f.Random.Decimal(min: 0, max: 100));

            var products = faker.Generate(40);
            dbContext.Products.AddRange(products);
            await dbContext.SaveChangesAsync();
        }
    }
}