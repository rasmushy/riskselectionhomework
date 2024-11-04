using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Homework.Api.Services;
using Homework.Api.Infrastructure;
using Homework.Api.Models;
using Homework.Api.Exceptions;
using Homework.Api.Tests.Helpers;  
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;
using Microsoft.Extensions.Logging;

namespace Homework.Api.Tests.Services
{
    /// <summary>
    /// Unit tests for the <see cref="ProductService"/> class.
    /// </summary>
    /// <author>Rasmus Hyyppä</author>
    public class ProductServiceTests
    {
        private readonly ProductService _productService;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductServiceTests"/> class, setting up mocks and the service under test.
        /// </summary>
        public ProductServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _loggerMock = new Mock<ILogger<ProductService>>();

            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            var options = Options.Create(new ProductSourceOptions
            {
                ProductSourceUrls = new List<string> { "https://dummyjson.com/products" }
            });

            _productService = new ProductService(new ProductApiClient(httpClient, options), _loggerMock.Object);
        }

        /// <summary>
        /// Tests that <see cref="ProductService.GetProductsAsync"/> returns an empty list when the API response is empty.
        /// </summary>
        [Fact]
        public async Task GetProductsAsync_ReturnsEmptyList_WhenApiReturnsEmptyResponse()
        {
            MockHttpHelper.SetupHttpClientResponse(_httpMessageHandlerMock, "{\"products\": []}", HttpStatusCode.OK);

            var result = await _productService.GetProductsAsync();
            Assert.Empty(result);
        }

        /// <summary>
        /// Tests that <see cref="ProductService.GetProductsAsync"/> throws a <see cref="JsonParseException"/> when the API response contains malformed JSON.
        /// </summary>
        [Fact]
        public async Task GetProductsAsync_ThrowsJsonParseException_WhenJsonIsMalformed()
        {
            MockHttpHelper.SetupHttpClientResponse(_httpMessageHandlerMock, "{ malformed_json: true }", HttpStatusCode.OK);

            await Assert.ThrowsAsync<JsonParseException>(() => _productService.GetProductsAsync());
        }

        /// <summary>
        /// Tests that <see cref="ProductService.GetProductsAsync"/> throws a <see cref="ServiceUnavailableException"/> when the HTTP request fails.
        /// </summary>
        [Fact]
        public async Task GetProductsAsync_ThrowsServiceUnavailableException_WhenHttpRequestFails()
        {
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Simulated HTTP request failure"));

            await Assert.ThrowsAsync<ServiceUnavailableException>(() => _productService.GetProductsAsync());
        }

        /// <summary>
        /// Tests that <see cref="ProductService.GetProductsAsync"/> ignores products with missing required fields in the response.
        /// </summary>
        [Fact]
        public async Task GetProductsAsync_IgnoresProductsWithMissingFields()
        {
            var responseContent = "{\"products\": [" +
                                  "{ \"id\": 1, \"title\": null, \"brand\": \"Brand A\", \"price\": 20, \"discountPercentage\": 15 }," +
                                  "{ \"id\": 2, \"title\": \"Product 2\", \"brand\": null, \"price\": 30, \"discountPercentage\": 10 }" +
                                  "]}";

            MockHttpHelper.SetupHttpClientResponse(_httpMessageHandlerMock, responseContent, HttpStatusCode.OK);

            var result = await _productService.GetProductsAsync();
            Assert.Empty(result);
        }

        /// <summary>
        /// Tests that <see cref="ProductService.GetProductsAsync"/> returns only valid products when the API response includes invalid products.
        /// </summary>
        [Fact]
        public async Task GetProductsAsync_ReturnsOnlyValidProducts_WhenSomeProductsAreInvalid()
        {
            var responseContent = "{\"products\": [" +
                "{ \"id\": 1, \"title\": \"Valid Product\", \"brand\": \"Brand A\", \"price\": 20, \"discountPercentage\": 15 }," +
                "{ \"id\": 2, \"title\": \"Invalid Product\", \"brand\": \"\", \"price\": 20, \"discountPercentage\": 5 }" +
                "]}";

            MockHttpHelper.SetupHttpClientResponse(_httpMessageHandlerMock, responseContent, HttpStatusCode.OK);

            var result = await _productService.GetProductsAsync();

            Assert.Single(result);
            Assert.Equal("Valid Product", result[0].Title);
        }
    }
}
