using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Homework.Api.Interfaces;
using Homework.Api.Models;
using Microsoft.Extensions.Options;

namespace Homework.Api.Infrastructure
{
    public class ProductApiClient : IProductApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ProductSourceOptions _options;

        public ProductApiClient(HttpClient httpClient, IOptions<ProductSourceOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<List<Product>> FetchProductsAsync()
        {
            var products = new List<Product>();

            foreach (var url in _options.ProductSourceUrls)
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var jsonDoc = JsonDocument.Parse(jsonString);

                    var sourceProducts = jsonDoc.RootElement.GetProperty("products").Deserialize<List<Product>>(new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (sourceProducts != null)
                    {
                        products.AddRange(sourceProducts);
                    }
                }
            }
            return products;
        }
    }
}
