using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Homework.Api.Infrastructure;
using Homework.Api.Models;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;
using Microsoft.Extensions.Logging;

namespace Homework.Api.Tests.Infrastructure
{
  /// <summary>
  /// Unit tests for the <see cref="ProductApiClient"/> class.
  /// <author>Rasmus Hyyppa</author>
  /// </summary>
  public class ProductApiClientTests
  {
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly Mock<ILogger<ProductApiClient>> _clientLoggerMock; 
    private readonly ProductApiClient _client;

    public ProductApiClientTests()
    {
      _httpMessageHandlerMock = new Mock<HttpMessageHandler>(); _clientLoggerMock = new Mock<ILogger<ProductApiClient>>(); 

      var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
      var options = Options.Create(new ProductSourceOptions
          {
          ProductSourceUrls = new List<string> { "https://test-url/products" }
          });

      _client = new ProductApiClient(httpClient, options, _clientLoggerMock.Object);
    }

    /// <summary>
    /// Verifies that <see cref="ProductApiClient.FetchProductsAsync"/> returns an empty list when the API response is empty.
    /// </summary>
    [Fact]
    public async Task FetchProductsAsync_ReturnsEmptyList_WhenResponseIsEmpty()
    {
      SetupHttpClientResponse("{\"products\": []}", HttpStatusCode.OK);

      var result = await _client.FetchProductsAsync();
      Assert.Empty(result);
    }

    /// <summary>
    /// Ensures that <see cref="ProductApiClient.FetchProductsAsync"/> logs a warning when no URLs are provided in the configuration.
    /// </summary>
    [Fact]
    public async Task FetchProductsAsync_LogsWarning_WhenNoUrlsProvided()
    {
      var options = Options.Create(new ProductSourceOptions { ProductSourceUrls = new List<string>() });
      var clientWithNoUrls = new ProductApiClient(new HttpClient(), options, _clientLoggerMock.Object);

      var result = await clientWithNoUrls.FetchProductsAsync();

      Assert.Empty(result);
      _clientLoggerMock.Verify(log => log.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No URLs provided")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    /// <summary>
    /// Ensures that <see cref="ProductApiClient.FetchProductsAsync"/> logs a warning and returns an empty list when a URL returns a non-success status code.
    /// </summary>
    [Fact]
    public async Task FetchProductsAsync_LogsWarning_WhenUrlReturnsNonSuccessStatusCode()
    {
      SetupHttpClientResponse("", HttpStatusCode.NotFound);

      var result = await _client.FetchProductsAsync();

      Assert.Empty(result);
      _clientLoggerMock.Verify(log => log.Log(
            LogLevel.Warning,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to fetch products from https://test-url/products with status code NotFound")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }

    private void SetupHttpClientResponse(string content, HttpStatusCode statusCode)
    {
      var response = new HttpResponseMessage(statusCode)
      {
        Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json")
      };

      _httpMessageHandlerMock.Protected()
        .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>())
        .ReturnsAsync(response);
    }
  }
}
