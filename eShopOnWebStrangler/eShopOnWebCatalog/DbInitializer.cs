using eShopOnWebCatalog.Models;

namespace eShopOnWebCatalog;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        // Ensure the database is created
        context.Database.EnsureCreated();

        // Seed CatalogBrands if none exist
        if (!context.CatalogBrands.Any())
        {
            context.CatalogBrands.AddRange(
                new CatalogBrand { Brand = "Brand A" },
                new CatalogBrand { Brand = "Brand B" }
            );
        }

        // Seed CatalogTypes if none exist
        if (!context.CatalogTypes.Any())
        {
            context.CatalogTypes.AddRange(
                new CatalogType { ItemType = "Type A" },
                new CatalogType { ItemType = "Type B" }
            );
        }

        // Seed CatalogItems if none exist
        if (!context.CatalogItems.Any())
        {
            context.CatalogItems.AddRange(
                new CatalogItem
                {
                    ItemName = "Sample Item 1",
                    Description = "This is a sample item.",
                    Price = 19.99M,
                    PictureUri = "http://example.com/item1.jpg",
                    CatalogTypeId = 1,
                    CatalogBrandId = 1
                },
                new CatalogItem
                {
                    ItemName = "Sample Item 2",
                    Description = "This is another sample item.",
                    Price = 29.99M,
                    PictureUri = "http://example.com/item2.jpg",
                    CatalogTypeId = 2,
                    CatalogBrandId = 2
                }
            );
        }

        context.SaveChanges();
    }
}
