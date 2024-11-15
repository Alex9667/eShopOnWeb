using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Migrations;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Messaging;
using System.Linq;

var serviceProvider = new ServiceCollection()
    .AddDbContext<InventoryDbContext>(options =>
        options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Inventory;Trusted_Connection=True;"))
    .BuildServiceProvider();

// Activates the latest migration if it isn't already in use
using (var scope = serviceProvider.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

    dbContext.Database.Migrate();
}

using (var context = new InventoryDbContext())
{
    var all = context.Inventories.ToList();

    foreach (var item in all)
    {
        Console.WriteLine($"ids: {item.ItemId}");
    }
}

InventoryMessageService messageService = new(new InventoryDbContext());

Console.WriteLine("Ready");
_ = Task.Run(() => messageService.ReceiveMessage("inventory", "inventoryRequestQueue"));
//await messageService.ReceiveMessage("inventory", "inventoryRequestQueue");

Console.WriteLine("YOOOO");
Console.ReadLine();

