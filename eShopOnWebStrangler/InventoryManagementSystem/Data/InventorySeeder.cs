using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManagementSystem.Migrations;
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

        Random random = new Random();

        while(id < 13)
        {
            int units = random.Next(20, 50);
            inventoryItems.Add(new InventoryModel(id, units, 0));

            id++;
        }

        return inventoryItems;
    }
}
