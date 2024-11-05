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
    }
}
