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
                new CatalogBrand (brand: "Brand A"),
                new CatalogBrand (brand: "Brand B")
            );
            context.SaveChanges();
        }


        if (!context.CatalogTypes.Any())
        {
            context.CatalogTypes.AddRange(
                new CatalogType ( type: "Type A" ),
                new CatalogType (type: "Type B")
            );
            context.SaveChanges();
        }


        if (!context.CatalogItems.Any())
        {
            context.CatalogItems.AddRange(
                new CatalogItem
                (
                    name: "Sample Item 1",
                    description: "This is a sample item.",
                    price: 19.99M,
                    pictureUri: "http://example.com/item1.jpg",
                    catalogTypeId: context.CatalogTypes.FirstOrDefault(c => c.Type == "Type A")?.Id ?? 0,
                    catalogBrandId: context.CatalogBrands.FirstOrDefault(c => c.Brand == "Brand A")?.Id ?? 0
                ),
                new CatalogItem
                (
                    name: "Sample Item 2",
                    description: "This is another sample item.",
                    price: 29.99M,
                    pictureUri: "http://example.com/item2.jpg",
                    catalogTypeId: context.CatalogTypes.FirstOrDefault(c => c.Type == "Type B")?.Id ?? 0,
                    catalogBrandId: context.CatalogBrands.FirstOrDefault(c => c.Brand == "Brand B")?.Id ?? 0
                )
            );
            context.SaveChanges();
        }
    }
}
