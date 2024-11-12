using Microsoft.EntityFrameworkCore;
using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Data;

internal class InventoryDbContext : DbContext
{
    public DbSet<InventoryModel> Inventories { get; set; }

    public InventoryDbContext()
    {

    }
    public InventoryDbContext(DbContextOptions<InventoryDbContext> dbContextOptions)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Inventory;Trusted_Connection=True;");
    }
}
