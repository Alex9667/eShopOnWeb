using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Migrations;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Services.Messaging;
using System.Linq;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();
var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
Console.WriteLine(env);

var connectionstring = env == "Docker" ? configuration.GetConnectionString("DockerConnection") : configuration.GetConnectionString("DefaultConnection");
Console.WriteLine("Connectionstring: " + connectionstring);
var serviceProvider = new ServiceCollection()
    .AddDbContext<InventoryDbContext>(options =>
        options.UseSqlServer(connectionstring))
    .BuildServiceProvider();


using (var scope = serviceProvider.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();

    dbContext.Database.Migrate();

    InventorySeeder seeder = new(dbContext);
    seeder.SeedDatabase();
}

using (var context = new InventoryDbContext())
{
    var all = context.Inventories.ToList();

    foreach (var item in all)
    {
        Console.WriteLine($"ids: {item.ItemId}");
    }
}
var rabbitMqSettings = new RabbitMqSettings();
configuration.GetSection("RabbitMq").Bind(rabbitMqSettings);
InventoryMessageService messageService = new(new InventoryDbContext(), rabbitMqSettings);

Console.WriteLine("Ready");

messageService.ReceiveMessage("inventory", "inventoryResponseQueue", "inventoryRequestQueue");

messageService.UpdateInventoryReceiver("inventory_update", "inventoryUpdateQueue");

Console.ReadLine();

