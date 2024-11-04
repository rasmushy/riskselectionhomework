using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Homework.Api.Infrastructure;
using Homework.Api.Models;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace Homework.Api.Tests.Infrastructure
{
    /// <summary>
    /// Unit tests for the <see cref="ProductApiClient"/> class.
    /// </summary>
    /// <author>Rasmus Hyyppä</author>
    public class ProductApiClientTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly ProductApiClient _client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductApiClientTests"/> class, setting up mocks and the client under test.
        /// </summary>
        public ProductApiClientTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            var options = Options.Create(new ProductSourceOptions
            {
                ProductSourceUrls = new List<string> { "https://dummyjson.com/products" }
            });

            _client = new ProductApiClient(httpClient, options);
        }

        /// <summary>
        /// Verifies that <see cref="ProductApiClient.FetchProductsAsync"/> returns an empty list when the API response is empty.
        /// </summary>
        [Fact]
        public async Task FetchProductsAsync_ReturnsEmptyList_WhenResponseIsEmpty()
        {
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"products\": []}")
                });

            var result = await _client.FetchProductsAsync();
            Assert.Empty(result);
        }

        /// <summary>
        /// Ensures that <see cref="ProductApiClient.FetchProductsAsync"/> throws an <see cref="HttpRequestException"/> if the API request fails.
        /// </summary>
        [Fact]
        public async Task FetchProductsAsync_ThrowsException_WhenRequestFails()
        {
            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException());

            await Assert.ThrowsAsync<HttpRequestException>(() => _client.FetchProductsAsync());
        }
    }
}
