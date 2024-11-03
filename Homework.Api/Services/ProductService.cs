using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Homework.Api.Interfaces;
using Homework.Api.Models;
using Microsoft.Extensions.Options;

namespace Homework.Api.Services
{
  public class ProductService : IProductService
  {
    private readonly HttpClient _httpClient;
    private readonly ProductSourceOptions _options;

    public ProductService(HttpClient httpClient, IOptions<ProductSourceOptions> options)
    {
      _httpClient = httpClient;
      _options = options.Value;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
      var products = new List<Product>();

      foreach (var url in _options.ProductSourceUrls)
      {
        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
          throw new HttpRequestException($"Request to {url} failed with status code {response.StatusCode}");
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(jsonString);

        var sourceProducts = jsonDoc.RootElement.GetProperty("products").Deserialize<List<Product>>(new JsonSerializerOptions
            {
            PropertyNameCaseInsensitive = true
            });

        if (sourceProducts != null)
        {
          products.AddRange(sourceProducts.Where(p => p.DiscountPercentage >= 10 && !string.IsNullOrEmpty(p.Brand)));
        }
      }

      Console.WriteLine("Filtered Products:");
      foreach (var product in products) 
      {
        Console.WriteLine($"{product.Title} - Discount: {product.DiscountPercentage}");
      }
      return products;
    }

    public Product GetTrendingProduct(List<Product> products)
    {
      return products.OrderByDescending(p => p.Rating).FirstOrDefault();
    }
  }
}
