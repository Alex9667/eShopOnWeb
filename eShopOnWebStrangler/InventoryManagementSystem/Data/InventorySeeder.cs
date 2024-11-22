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

        int id = 1;


        while(id < 13)
        {
            int units = 500;
            inventoryItems.Add(new InventoryModel(id, units, 0));

            id++;
        }

        return inventoryItems;
    }
}
