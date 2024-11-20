using eShopOnWebCatalog.Models;

namespace eShopOnWebCatalog;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        
        context.Database.EnsureCreated();

        
        if (!context.CatalogBrands.Any())
        {
            context.CatalogBrands.AddRange(
                new CatalogBrand { Brand = "Brand A" },
                new CatalogBrand { Brand = "Brand B" }
            );
        }

        
        if (!context.CatalogTypes.Any())
        {
            context.CatalogTypes.AddRange(
                new CatalogType { ItemType = "Type A" },
                new CatalogType { ItemType = "Type B" }
            );
        }

        
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
