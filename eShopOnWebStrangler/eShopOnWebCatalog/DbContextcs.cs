using eShopOnWebCatalog.Models;
using Microsoft.EntityFrameworkCore;

namespace eShopOnWebCatalog;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {

    }

    public DbSet<CatalogItem> CatalogItems {  get; set; }
    public DbSet<CatalogBrand> CatalogBrands {  get; set; }
    public DbSet<CatalogType> CatalogTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<CatalogBrand>().HasKey(cb => cb.BrandID);
        modelBuilder.Entity<CatalogType>().HasKey(ct => ct.TypeID);
        modelBuilder.Entity<CatalogItem>().HasKey(ci => ci.ItemID);

        modelBuilder.Entity<CatalogItem>()
       .Property(ci => ci.Price)
       .HasColumnType("DECIMAL(6,2)"); 

        modelBuilder.Entity<CatalogItem>()
            .HasOne(ci => ci.CatalogType)
            .WithMany()
            .HasForeignKey(ci => ci.CatalogTypeId);

        modelBuilder.Entity<CatalogItem>()
            .HasOne(ci =>ci.CatalogBrand)
            .WithMany()
            .HasForeignKey(ci => ci.CatalogBrandId);
    }
}
