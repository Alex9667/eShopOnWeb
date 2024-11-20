using eShopOnWebCatalog.Models;
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
                new CatalogBrand { Brand = "Brand A" },
                new CatalogBrand { Brand = "Brand B" }
            );
            context.SaveChanges();
        }


        if (!context.CatalogTypes.Any())
        {
            context.CatalogTypes.AddRange(
                new CatalogType { ItemType = "Type A" },
                new CatalogType { ItemType = "Type B" }
            );
            context.SaveChanges();
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
                    CatalogTypeId = context.CatalogTypes.FirstOrDefault(c => c.ItemType == "Type A")?.TypeID ?? 0,
                    CatalogBrandId = context.CatalogBrands.FirstOrDefault(c => c.Brand == "Brand A")?.BrandID ?? 0
                },
                new CatalogItem
                {
                    ItemName = "Sample Item 2",
                    Description = "This is another sample item.",
                    Price = 29.99M,
                    PictureUri = "http://example.com/item2.jpg",
                    CatalogTypeId = context.CatalogTypes.FirstOrDefault(c => c.ItemType == "Type B")?.TypeID ?? 0,
                    CatalogBrandId = context.CatalogBrands.FirstOrDefault(c => c.Brand == "Brand B")?.BrandID ?? 0
                }
            );
            context.SaveChanges();
        }
    }
}
