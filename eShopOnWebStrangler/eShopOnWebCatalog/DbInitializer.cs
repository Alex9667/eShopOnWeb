using eShopOnWebCatalog.Entities;
using Microsoft.EntityFrameworkCore;

namespace eShopOnWebCatalog;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {

        context.Database.Migrate();


        if (!context.CatalogBrands.Any())
        {
            context.CatalogBrands.AddRange(
                new("Azure"),
                new(".NET"),
                new("Visual Studio"),
                new("SQL Server"),
                new("Other")
            );
            context.SaveChanges();
        }


        if (!context.CatalogTypes.Any())
        {
            context.CatalogTypes.AddRange(
                new("Mug"),
                new("T-Shirt"),
                new("Sheet"),
                new("USB Memory Stick")
            );
            context.SaveChanges();
        }


        if (!context.CatalogItems.Any())
        {
            context.CatalogItems.AddRange(
                new(2, 2, ".NET Bot Black Sweatshirt", ".NET Bot Black Sweatshirt", 19.5M, "http://catalogbaseurltobereplaced/images/products/1.png"),
                new(1, 2, ".NET Black & White Mug", ".NET Black & White Mug", 8.50M, "http://catalogbaseurltobereplaced/images/products/2.png"),
                new(2, 5, "Prism White T-Shirt", "Prism White T-Shirt", 12, "http://catalogbaseurltobereplaced/images/products/3.png"),
                new(2, 2, ".NET Foundation Sweatshirt", ".NET Foundation Sweatshirt", 12, "http://catalogbaseurltobereplaced/images/products/4.png"),
                new(3, 5, "Roslyn Red Sheet", "Roslyn Red Sheet", 8.5M, "http://catalogbaseurltobereplaced/images/products/5.png"),
                new(2, 2, ".NET Blue Sweatshirt", ".NET Blue Sweatshirt", 12, "http://catalogbaseurltobereplaced/images/products/6.png"),
                new(2, 5, "Roslyn Red T-Shirt", "Roslyn Red T-Shirt", 12, "http://catalogbaseurltobereplaced/images/products/7.png"),
                new(2, 5, "Kudu Purple Sweatshirt", "Kudu Purple Sweatshirt", 8.5M, "http://catalogbaseurltobereplaced/images/products/8.png"),
                new(1, 5, "Cup<T> White Mug", "Cup<T> White Mug", 12, "http://catalogbaseurltobereplaced/images/products/9.png"),
                new(3, 2, ".NET Foundation Sheet", ".NET Foundation Sheet", 12, "http://catalogbaseurltobereplaced/images/products/10.png"),
                new(3, 2, "Cup<T> Sheet", "Cup<T> Sheet", 8.5M, "http://catalogbaseurltobereplaced/images/products/11.png"),
                new(2, 5, "Prism White TShirt", "Prism White TShirt", 12, "http://catalogbaseurltobereplaced/images/products/12.png")
            );
            context.SaveChanges();
        }
    }
}
