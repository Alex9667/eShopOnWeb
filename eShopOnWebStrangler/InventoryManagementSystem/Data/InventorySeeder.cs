using InventoryManagementSystem.Models;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManagementSystem.Data;
internal class InventorySeeder
{
    public void SeedDatabase()
    {
        using (var context = new InventoryDbContext())
        {
            if(context.Inventories.ToList().IsNullOrEmpty())
            {
                List<InventoryModel> inventoryItems = getInventoryItems();
                foreach (var item in inventoryItems)
                {
                    context.Inventories.Add(item);
                }
                context.SaveChanges();
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
