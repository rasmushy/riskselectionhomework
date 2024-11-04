using System.Collections.Generic;
using System.Threading.Tasks;
using Homework.Api.Controllers;
using Homework.Api.Models;
using Homework.Api.Interfaces;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductControllerTests"/> class, setting up mocks and the controller under test.
        /// </summary>
        public ProductControllerTests()
        {
            _productServiceMock = new Mock<IProductService>();
            _loggerMock = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_productServiceMock.Object, _loggerMock.Object);
        }

        /// <summary>
        /// Tests that <see cref="ProductsController.GetProducts"/> returns a <see cref="NotFoundObjectResult"/> with a message if the product list is empty.
        /// </summary>
        [Fact]
        public async Task GetProducts_ReturnsNotFound_WhenServiceReturnsEmpty()
        {
            _productServiceMock.Setup(service => service.GetProductsAsync()).ReturnsAsync(new List<Product>());

            var result = await _controller.GetProducts();
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var returnedMessage = Assert.IsType<string>(notFoundResult.Value);
            Assert.Equal("No products available.", returnedMessage);
        }

        /// <summary>
        /// Tests that <see cref="ProductsController.GetProducts"/> returns <see cref="NotFoundObjectResult"/> when the product service returns null.
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
        /// Tests that <see cref="ProductsController.GetProducts"/> logs an error and returns an <see cref="ObjectResult"/> if the service throws an exception.
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
    }
}
