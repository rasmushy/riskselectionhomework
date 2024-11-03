using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Homework.Api.Controllers;
using Homework.Api.Models;
using Homework.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xunit;

/// <summary>
/// Tests for the ProductsController class to ensure API endpoints return expected results
/// <author>Rasmus Hyyppä</author>
/// </summary>
public class ProductControllerTests
{
    private readonly ProductsController _controller;

    public ProductControllerTests()
    {
        var options = Options.Create(new ProductSourceOptions { ProductSourceUrls = new List<string> { "https://mockurl.com" } });
        var httpClient = new HttpClient(new MockHttpMessageHandler());
        var productService = new ProductService(httpClient, options);
        _controller = new ProductsController(productService);
    }

    /// <summary>
    /// Tests that GetProducts returns an OkObjectResult containing a non-empty list of products.
    /// </summary>
    /// <returns>Asserts that the response contains a list of products with at least one item.</returns>
    [Fact]
    public async Task GetProducts_ReturnsOkResultWithProducts()
    {
        var result = await _controller.GetProducts();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProducts = Assert.IsType<List<Product>>(okResult.Value);
        Assert.NotEmpty(returnedProducts);
    }

    /// <summary>
    /// Tests that GetProducts returns an OkObjectResult containing an empty list
    /// when no products are available.
    /// </summary>
    /// <returns>Asserts that the response contains an empty list.</returns>
    [Fact]
    public async Task GetProducts_ReturnsEmptyListWhenNoProducts()
    {
        var httpClient = new HttpClient(new MockHttpMessageHandler(returnEmptyList: true));
        var productService = new ProductService(httpClient, Options.Create(new ProductSourceOptions()));
        var controller = new ProductsController(productService);

        var result = await controller.GetProducts();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProducts = Assert.IsType<List<Product>>(okResult.Value);
        Assert.Empty(returnedProducts);
    }

    /// <summary>
    /// Tests that GetProducts returns products with expected field values populated.
    /// </summary>
    /// <returns>Asserts that the returned products have all required fields set correctly.</returns>
    [Fact]
    public async Task GetProducts_ReturnsProductsWithExpectedFields()
    {
        var result = await _controller.GetProducts();

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProducts = Assert.IsType<List<Product>>(okResult.Value);
        var product = returnedProducts[0];

        Assert.NotNull(product.Title);
        Assert.NotNull(product.Brand);
        Assert.Equal(30, product.Price);
        Assert.Equal(15, product.DiscountPercentage);
        Assert.Equal(4.5f, product.Rating);
    }

    /// <summary>
    /// Tests that GetProducts returns a filtered list of products based on discount criteria.
    /// Only products with a DiscountPercentage of 10 or higher should be returned.
    /// </summary>
    /// <returns>Asserts that only products meeting the discount criteria are returned.</returns>
    [Fact]
    public async Task GetProducts_ReturnsFilteredProductsBasedOnCriteria()
    {
        var result = await _controller.GetProducts();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProducts = Assert.IsType<List<Product>>(okResult.Value);

        Assert.Single(returnedProducts);
        Assert.Equal("Product 2", returnedProducts[0].Title);
        Assert.True(returnedProducts[0].DiscountPercentage >= 10);
    }

    /// <summary>
    /// MockHttpMessageHandler simulates an HTTP response for ProductService.
    /// Returns either a predefined list of products or an empty list based on configuration.
    /// </summary>
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly bool _returnEmptyList;

        /// <param name="returnEmptyList">Determines whether to return an empty list of products.</param>
        public MockHttpMessageHandler(bool returnEmptyList = false)
        {
            _returnEmptyList = returnEmptyList;
        }

        /// <summary>
        /// Sends a mocked HTTP response with predefined JSON data.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="cancellationToken">A cancellation token for the async operation.</param>
        /// <returns>An HTTP response message with JSON content.</returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            var products = _returnEmptyList
                ? new { products = new List<Product>() }
                : new
                {
                    products = new List<Product>
                    {
                        new Product { Id = 1, Title = "Product 1", Brand = "Brand A", Price = 20, DiscountPercentage = 5, Rating = 3.69f },
                        new Product { Id = 2, Title = "Product 2", Brand = "Brand B", Price = 30, DiscountPercentage = 15, Rating = 4.5f }
                    }
                };
                
            var json = JsonSerializer.Serialize(products);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}
