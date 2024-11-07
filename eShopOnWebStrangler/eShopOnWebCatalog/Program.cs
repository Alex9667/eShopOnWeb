using System.Data.Entity;
using eShopOnWebCatalog;
using eShopOnWebCatalog.Data;
using eShopOnWebCatalog.Entities;
using eShopOnWebCatalog.Infrastructure;
using eShopOnWebCatalog.Interfaces;
using eShopOnWebCatalog.Services;
using eShopOnWebCatalog.Services.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.PublicApi;
using Microsoft.OpenApi.Models;
using MinimalApi.Endpoint.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpoints();
builder.Services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

builder.Services.Configure<CatalogSettings>(builder.Configuration);

builder.Services.AddScoped<IMessagingService, CatalogMessageService>();
//builder.Services.AddHostedService<CatalogMessageService>();

var catalogSettings = builder.Configuration.Get<CatalogSettings>() ?? new CatalogSettings();
builder.Services.AddSingleton<IUriComposer>(new UriComposer(catalogSettings));
//builder.Services.AddScoped(typeof(IMessagingService),typeof(CatalogMessageService));
//builder.Services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));
//builder.Services.AddScoped<ITokenClaimsService, IdentityTokenClaimService>();
var connectionstring = builder.Configuration.GetConnectionString("CatalogConnection");
builder.Services.AddDbContext<CatalogContext>(
    options => options.UseSqlServer(connectionstring)
);

var configSection = builder.Configuration.GetRequiredSection(BaseUrlConfiguration.CONFIG_NAME);
builder.Services.Configure<BaseUrlConfiguration>(configSection);
var baseUrlConfig = configSection.Get<BaseUrlConfiguration>();

builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Configuration.AddEnvironmentVariables();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
//    c.EnableAnnotations();
//    c.SchemaFilter<CustomSchemaFilters>();
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
//                      Enter 'Bearer' [space] and then your token in the text input below.
//                      \r\n\r\nExample: 'Bearer 12345abcdef'",
//        Name = "Authorization",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });

//    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type = ReferenceType.SecurityScheme,
//                    Id = "Bearer"
//                },
//                Scheme = "oauth2",
//                Name = "Bearer",
//                In = ParameterLocation.Header,

//            },
//            new List<string>()
//        }
//    });
//});
var app = builder.Build();
var a  = app.Services;
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
}

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
