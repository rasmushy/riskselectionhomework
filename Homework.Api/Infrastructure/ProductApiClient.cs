using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Homework.Api.Interfaces;
using Homework.Api.Models;
using Homework.Api.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Homework.Api.Infrastructure
{
    /// <summary>
    /// Manages communication with external product APIs, using <see cref="ApiResponseHandler"/>
    /// to parse and validate the response data.
    /// </summary>
    public class ProductApiClient : IProductApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ProductSourceOptions _options;
        private readonly ILogger<ProductApiClient> _logger;

        public ProductApiClient(HttpClient httpClient, IOptions<ProductSourceOptions> options, ILogger<ProductApiClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Product>> FetchProductsAsync()
        {
            if (_options.ProductSourceUrls == null || !_options.ProductSourceUrls.Any())
            {
                _logger.LogWarning("No URLs provided in ProductSourceUrls. Ensure that configuration is set correctly.");
                return new List<Product>();
            }

            var products = new List<Product>();

            foreach (var url in _options.ProductSourceUrls)
            {
                var productList = await FetchProductsFromUrlAsync(url);
                products.AddRange(productList);
            }

            return products;
        }

        /// <summary>
        /// Fetches products from a single URL and processes the response.
        /// </summary>
        private async Task<List<Product>> FetchProductsFromUrlAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                return await ProcessResponseAsync(response, url);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error when fetching products from {Url}", url);
                return new List<Product>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when fetching products from {Url}", url);
                return new List<Product>();
            }
        }

        /// <summary>
        /// Processes the HTTP response and deserializes the product list.
        /// </summary>
        private async Task<List<Product>> ProcessResponseAsync(HttpResponseMessage response, string url)
        {
            if (response.IsSuccessStatusCode)
            {
                return await ApiResponseHandler.HandleResponse(response);
            }
            else
            {
                _logger.LogWarning("Failed to fetch products from {Url} with status code {StatusCode}", url, response.StatusCode);
                return new List<Product>();
            }
        }
    }
}
