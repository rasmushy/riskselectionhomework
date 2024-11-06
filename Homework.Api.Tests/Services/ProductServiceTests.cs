using System.Collections.Generic;
using System.Threading.Tasks;
using Homework.Api.Interfaces;
using Homework.Api.Models;
using Homework.Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Homework.Api.Tests.Services
{
  /// <summary>
  /// Unit tests for the <see cref="ProductService"/> class.
  /// </summary>
  /// <author>Rasmus Hyyppä</author>
  public class ProductServiceTests
  {
    private readonly Mock<IProductApiClient> _productApiClientMock;
    private readonly ProductService _productService;
    private readonly Mock<ILogger<ProductService>> _loggerMock;

    public ProductServiceTests()
    {
      _productApiClientMock = new Mock<IProductApiClient>();
      _loggerMock = new Mock<ILogger<ProductService>>();
      _productService = new ProductService(_productApiClientMock.Object, _loggerMock.Object);
    }

    /// <summary>
    /// Tests that <see cref="ProductService.GetProductsAsync"/> returns an empty list when the API response is empty.
    /// </summary>
    [Fact]
    public async Task GetProductsAsync_ReturnsEmptyList_WhenApiReturnsEmptyResponse()
    {
      _productApiClientMock.Setup(client => client.FetchProductsAsync())
        .ReturnsAsync(new List<Product>());

      var result = await _productService.GetProductsAsync();
      Assert.Empty(result);
    }

    /// <summary>
    /// Tests that <see cref="ProductService.GetProductsAsync"/> logs the correct count of products retrieved on a successful response.
    /// </summary>
    [Fact]
    public async Task GetProductsAsync_LogsProductCount_OnSuccessfulFetch()
    {
      var products = new List<Product> { new Product { Id = 1, Title = "Product" } };
      _productApiClientMock.Setup(client => client.FetchProductsAsync())
        .ReturnsAsync(products);

      await _productService.GetProductsAsync();

      _loggerMock.Verify(log => log.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Fetched 1 products from the external API.")),
            null,
            It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
    }
  }
}
