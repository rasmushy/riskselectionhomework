using Homework.Api.Interfaces;
using Homework.Api.Models;
using Homework.Api.Services;
using Homework.Api.Infrastructure;
using Homework.Api.Middleware;
using Microsoft.Extensions.Http.Resilience;

var builder = WebApplication.CreateBuilder(args);

// Configure options from appsettings.json to add more external API's
builder.Services.Configure<ProductSourceOptions>(builder.Configuration.GetSection("ProductSourceOptions"));

builder.Services.AddHttpClient<IProductApiClient, ProductApiClient>()
    .AddStandardResilienceHandler();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddControllers();

builder.Services.AddCors(options => options.AddPolicy("AllowReactApp", policy => 
    policy.WithOrigins("http://localhost:3000")
          .AllowAnyHeader()
          .AllowAnyMethod()));

var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseCors("AllowReactApp");
app.MapControllers();
app.Run();
