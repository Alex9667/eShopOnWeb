using eShopOnWebCatalog.Models;
using eShopOnWebCatalog;  
using Microsoft.EntityFrameworkCore;
using System;

namespace InsertTestDB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=CatalogDB;User Id=sa;Password=Ladida.12;TrustServerCertificate=True;MultipleActiveResultSets=true;");

            
            using (var context = new ApplicationDbContext(optionsBuilder.Options))
            {
                InsertData(context);
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static void InsertData(ApplicationDbContext context)
        {
            
            var newBrand = new CatalogBrand
            {
                Brand = "Sample Brand"
            };
            context.CatalogBrands.Add(newBrand);
            context.SaveChanges(); 

           
            var newType = new CatalogType
            {
                ItemType = "Sample Type"
            };
            context.CatalogTypes.Add(newType);
            context.SaveChanges(); 

            var newItem = new CatalogItem
            {
                ItemName = "Sample Item",
                Description = "This is a sample item.",
                Price = 99.99M,
                PictureUri = "http://example.com/sample.jpg",
                CatalogTypeId = newType.TypeID,  
                CatalogBrandId = newBrand.BrandID  
            };

            
            context.CatalogItems.Add(newItem);

            
            context.SaveChanges();

            Console.WriteLine("Item inserted successfully.");
        }

    }
}
