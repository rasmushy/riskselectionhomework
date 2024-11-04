using Homework.Api.Interfaces;
using Homework.Api.Models;
using Homework.Api.Services;
using Homework.Api.Infrastructure;
using Homework.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ProductSourceOptions>(builder.Configuration.GetSection("ProductSourceOptions"));
builder.Services.AddHttpClient<IProductApiClient, ProductApiClient>(); 
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddControllers();
builder.Services.AddCors(options => options.AddPolicy("AllowReactApp", policy => 
    policy.WithOrigins("http://localhost:3000")
          .AllowAnyHeader()
          .AllowAnyMethod()));

var app = builder.Build();

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();
app.Run();
