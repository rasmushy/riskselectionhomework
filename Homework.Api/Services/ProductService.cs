using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homework.Api.Interfaces;
using Homework.Api.Models;
using Homework.Api.Exceptions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Homework.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductApiClient _productApiClient;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductApiClient productApiClient, ILogger<ProductService> logger)
        {
            _productApiClient = productApiClient ?? throw new ArgumentNullException(nameof(productApiClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            _logger.LogInformation("Starting product fetch from external API.");

            try
            {
                var products = await _productApiClient.FetchProductsAsync();

                var filteredProducts = FilterValidProducts(products);
                _logger.LogInformation("{Count} products passed the filter criteria.", filteredProducts.Count);

                return filteredProducts;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse products from API response.");
                throw new JsonParseException("Invalid JSON format in product data", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Failed to fetch products due to a network issue.");
                throw new ServiceUnavailableException("External service is unavailable", ex);
            }
        }

        private List<Product> FilterValidProducts(IEnumerable<Product> products)
        {
            return products
                .Where(p => 
                    p.DiscountPercentage >= 10 && 
                    !string.IsNullOrEmpty(p.Brand) && 
                    !string.IsNullOrEmpty(p.Title))
                .ToList();
        }
    }
}
