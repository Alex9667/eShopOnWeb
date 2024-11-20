using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Services.Messaging;

namespace InventoryManagementSystem.Models;
internal class InventoryRepo
{
    public void ReduceInventoryAmount(List<InventoryModel> items)
    {
        using (var context = new InventoryDbContext())
        {
            foreach (var item in items)
            {
                var inventoryItem = context.Inventories.FirstOrDefault(i => i.ItemId == item.ItemId);

                if(inventoryItem != null)
                {
                    inventoryItem.Units -= item.Units;
                    context.SaveChanges();
                    Console.WriteLine($"Item {item.ItemId} has {inventoryItem.Units} units left");
                }
                else
                {
                    Console.WriteLine($"Item {item.ItemId} not found");
                }
            }
        }
    }
}
