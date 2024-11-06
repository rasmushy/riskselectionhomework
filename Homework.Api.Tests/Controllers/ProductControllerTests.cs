using System.Collections.Generic;
using System.Threading.Tasks;
using Homework.Api.Controllers;
using Homework.Api.Models;
using Homework.Api.Interfaces;
using Homework.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Homework.Api.Tests.Controllers
{
  /// <summary>
  /// Unit tests for the <see cref="ProductsController"/> class.
  /// </summary>
  /// <author>Rasmus Hyyppä</author>
  public class ProductControllerTests
  {
    private readonly ProductsController _controller;
    private readonly Mock<IProductService> _productServiceMock;
    private readonly Mock<ILogger<ProductsController>> _loggerMock;

    public ProductControllerTests()
    {
      _productServiceMock = new Mock<IProductService>();
      _loggerMock = new Mock<ILogger<ProductsController>>();
      _controller = new ProductsController(_productServiceMock.Object, _loggerMock.Object);
    }

    /// <summary>
    /// Verifies that <see cref="ProductsController.GetProducts"/> returns a <see cref="NotFoundObjectResult"/> with an appropriate message if the product list is empty.
    /// </summary>
    [Fact]
    public async Task GetProducts_ReturnsNotFound_WhenServiceReturnsEmpty()
    {
      _productServiceMock.Setup(service => service.GetProductsAsync()).ReturnsAsync(new List<Product>());

      var result = await _controller.GetProducts();
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
      Assert.Equal("No products available.", notFoundResult.Value);
    }

    /// <summary>
    /// Ensures that <see cref="ProductsController.GetProducts"/> returns a <see cref="NotFoundObjectResult"/> when the product service returns null.
    /// </summary>
    [Fact]
    public async Task GetProducts_ReturnsNotFound_WhenServiceReturnsNull()
    {
      _productServiceMock.Setup(service => service.GetProductsAsync()).ReturnsAsync((List<Product>?)null);

      var result = await _controller.GetProducts();
      var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
      Assert.Equal("No products available.", notFoundResult.Value);
    }

    /// <summary>
    /// Verifies that <see cref="ProductsController.GetProducts"/> returns an <see cref="OkObjectResult"/> with a list of products when products are available.
    /// </summary>
    [Fact]
    public async Task GetProducts_ReturnsOk_WithProducts_WhenProductsAvailable()
    {
      var products = new List<Product>
      {
        new Product { Id = 1, Title = "Product 1", Brand = "Brand A", Price = 20, DiscountPercentage = 15 },
            new Product { Id = 2, Title = "Product 2", Brand = "Brand B", Price = 30, DiscountPercentage = 10 }
      };

      _productServiceMock.Setup(service => service.GetProductsAsync()).ReturnsAsync(products);

      var result = await _controller.GetProducts();
      var okResult = Assert.IsType<OkObjectResult>(result.Result);
      var returnedProducts = Assert.IsType<List<Product>>(okResult.Value);
      Assert.Equal(products.Count, returnedProducts.Count);
    }

    /// <summary>
    /// Ensures that <see cref="ProductsController.GetProducts"/> logs an error and returns a generic <see cref="ObjectResult"/> when the service throws an exception.
    /// </summary>
    [Fact]
    public async Task GetProducts_LogsError_WhenServiceThrowsException()
    {
      _productServiceMock.Setup(service => service.GetProductsAsync()).ThrowsAsync(new System.Exception("Service error"));

      var result = await _controller.GetProducts();

      Assert.NotNull(result);
      Assert.IsType<ObjectResult>(result.Result);

      _loggerMock.Verify(log => log.Log(
            LogLevel.Error,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An unexpected error occurred while fetching products.")),
            It.IsAny<Exception>(),
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    /// <summary>
    /// Verifies that <see cref="ProductsController.GetProducts"/> returns an appropriate message when no products are available due to external API issues.
    /// </summary>
    [Fact]
    public async Task GetProducts_ReturnsServiceUnavailable_WhenExternalApiFails()
    {
      _productServiceMock.Setup(service => service.GetProductsAsync())
        .ThrowsAsync(new ServiceUnavailableException("Service unavailable", new Exception("Inner exception")));

      var result = await _controller.GetProducts();
      var objectResult = Assert.IsType<ObjectResult>(result.Result);

      Assert.Equal(503, objectResult.StatusCode);
      Assert.Equal("Service is currently unavailable. Please try again later.", objectResult.Value);
    }
  }
}
