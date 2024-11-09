using System.Collections.Generic;
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
    }
}
