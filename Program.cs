using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using CatalogApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Configure Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost:6379"));
builder.Services.AddTransient<ISmartCacheService, SmartCacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
