using Homework.Api.Interfaces;
using Homework.Api.Models;
using Homework.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ProductSourceOptions>(builder.Configuration.GetSection("ProductSourceOptions"));
builder.Services.AddHttpClient<IProductService, ProductService>();

builder.Services.AddControllers();
builder.Services.AddCors(options => options.AddPolicy("AllowReactApp", policy => 
    policy.WithOrigins("http://localhost:3000")
          .AllowAnyHeader()
          .AllowAnyMethod()));

var app = builder.Build();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();
app.Run();
