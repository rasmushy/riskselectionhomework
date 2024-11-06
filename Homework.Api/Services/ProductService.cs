using System.Collections.Generic;
using System.Threading.Tasks;
using Homework.Api.Interfaces;
using Homework.Api.Models;
using Homework.Api.Exceptions;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Homework.Api.Services
{
    /// <summary>
    /// Coordinates the retrieval of products from an external API, using <see cref="ProductApiClient"/>
    /// to fetch and return the final product data list.
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
                _logger.LogInformation("Fetched {Count} products from the external API.", products.Count);

                if (products.Count == 0)
                {
                    _logger.LogWarning("No products were returned from the external API.");
                }
                else
                {
                    _logger.LogInformation("{Count} products successfully retrieved.", products.Count);
                }

                return products;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error: Failed to parse products from the external API response.");
                throw new JsonParseException("Invalid JSON format in product data", ex);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error: Failed to fetch products due to a network issue while accessing external API.");
                throw new ServiceUnavailableException("External service is unavailable", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during product fetch.");
                throw; 
            }
        }
    }
}
