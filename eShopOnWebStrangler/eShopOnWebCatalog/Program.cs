using System.Data.Entity;
using eShopOnWebCatalog;
using eShopOnWebCatalog.Data;
using eShopOnWebCatalog.Models;
using eShopOnWebCatalog.Entities;
using eShopOnWebCatalog.Infrastructure;
using eShopOnWebCatalog.Interfaces;
using eShopOnWebCatalog.Services;
using eShopOnWebCatalog.Services.Messaging;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Endpoint.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpoints();
builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.Configure<CatalogSettings>(builder.Configuration);

builder.Services.AddScoped<IMessagingService, CatalogMessageService>();
builder.Services.AddHostedService<BackgroundMessegingService>();


var catalogSettings = builder.Configuration.Get<CatalogSettings>() ?? new CatalogSettings();
builder.Services.AddSingleton<IUriComposer>(new UriComposer(catalogSettings));

var connectionstring = builder.Configuration.GetConnectionString("MonolithCatalogConnection");
builder.Services.AddDbContext<CatalogContext>(
    options => options.UseSqlServer(connectionstring)
);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CatalogConnection")));

var configSection = builder.Configuration.GetRequiredSection(BaseUrlConfiguration.CONFIG_NAME);
builder.Services.Configure<BaseUrlConfiguration>(configSection);
var baseUrlConfig = configSection.Get<BaseUrlConfiguration>();

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Configuration.AddEnvironmentVariables();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Database Initialization Logic
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        //var catalogContext = services.GetRequiredService<CatalogContext>();
        //catalogContext.Database.Migrate();
        Console.WriteLine($"Seeding database: {builder.Configuration.GetConnectionString("CatalogConnection")}");
        var applicationDbContext = services.GetRequiredService<ApplicationDbContext>();
        applicationDbContext.Database.Migrate();

        // Call the initializer to seed data
        DbInitializer.Initialize(applicationDbContext);

        Console.WriteLine("Databases initialized successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while initializing the database: {ex.Message}");
        throw;
    }
}


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
//}


app.MapGet("/health", context =>
        context.Response.WriteAsync("My API is healthy!"))
   .WithName("HealthCheck")
   .WithDisplayName("HealthCheck");

app.MapGet("/hello", () => "Hello!");

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapEndpoints();

app.Run();
