﻿using System;
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

await messageService.ReceiveMessage("inventory", "inventoryRequestQueue");

Console.ReadLine();
