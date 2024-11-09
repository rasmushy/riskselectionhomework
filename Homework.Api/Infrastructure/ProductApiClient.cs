using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Homework.Api.Interfaces;
using Homework.Api.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Homework.Api.Infrastructure
{
    public class ProductApiClient : IProductApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ProductSourceOptions _options;
        private readonly ILogger<ProductApiClient> _logger;
        private readonly ParsingStrategyResolver _parsingStrategyResolver;

        public ProductApiClient(
            HttpClient httpClient,
            IOptions<ProductSourceOptions> options,
            ILogger<ProductApiClient> logger,
            ParsingStrategyResolver parsingStrategyResolver)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _parsingStrategyResolver = parsingStrategyResolver ?? throw new ArgumentNullException(nameof(parsingStrategyResolver));
        }

        public async Task<List<Product>> FetchProductsAsync()
        {
          var products = new List<Product>();

          foreach (var source in _options.Sources)
          {
            var strategy = _parsingStrategyResolver.GetStrategy(source.ParsingStrategy);
            if (strategy == null)
            {
              _logger.LogWarning("No parsing strategy found for URL {Url} with strategy {Strategy}.", source.Url, source.ParsingStrategy);
              continue;
            }

            _logger.LogInformation("Using strategy {Strategy} for URL {Url}.", source.ParsingStrategy, source.Url);

            var productList = await FetchProductsFromUrlAsync(source.Url, strategy);

            _logger.LogInformation("Fetched {Count} products from {Url}.", productList.Count, source.Url);
            products.AddRange(productList);
          }

          _logger.LogInformation("Total products fetched: {Count}", products.Count);
          return products;
        }

        private async Task<List<Product>> FetchProductsFromUrlAsync(string url, IApiResponseParsingStrategy parsingStrategy)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await parsingStrategy.ParseResponseAsync(response);
        }
    }
}
