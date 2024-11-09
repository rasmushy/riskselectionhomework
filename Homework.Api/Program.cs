using Homework.Api.Interfaces;
using Homework.Api.Models;
using Homework.Api.Services;
using Homework.Api.Infrastructure;
using Homework.Api.Middleware;
using Homework.Api.Infrastructure.ParsingStrategies;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Configure ProductSourceOptions from appsettings.json
builder.Services.Configure<ProductSourceOptions>(builder.Configuration.GetSection("ProductSourceOptions"));

builder.Services.AddTransient<IApiResponseParsingStrategy, DummyJsonParsingStrategy>();
builder.Services.AddTransient<IApiResponseParsingStrategy, FakeStoreApiParsingStrategy>();
builder.Services.AddScoped<ParsingStrategyResolver>();

builder.Services.AddHttpClient<IProductApiClient, ProductApiClient>().AddStandardResilienceHandler();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowReactApp");

app.UseMiddleware<ErrorHandlerMiddleware>();
app.MapControllers();
app.Run();
