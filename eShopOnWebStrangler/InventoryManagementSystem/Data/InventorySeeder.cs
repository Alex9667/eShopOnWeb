using InventoryManagementSystem.Models;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManagementSystem.Data;
internal class InventorySeeder
{
    private readonly InventoryDbContext _context;

    public InventorySeeder(InventoryDbContext context)
    {
        _context = context;
    }
    public void SeedDatabase()
    {
        using (_context)
        {
            if(_context.Inventories.ToList().IsNullOrEmpty())
            {
                List<InventoryModel> inventoryItems = getInventoryItems();
                foreach (var item in inventoryItems)
                {
                    _context.Inventories.Add(item);
                }
                _context.SaveChanges();
            }
        }
    }

    private List<InventoryModel> getInventoryItems()
    {
        List<InventoryModel> inventoryItems = new List<InventoryModel>();

        int catalogItemId = 1;


        while(catalogItemId < 13)
        {
            int units = 500;
            inventoryItems.Add(new InventoryModel(catalogItemId, units, 0));

            catalogItemId++;
        }

        return inventoryItems;
    }
}
