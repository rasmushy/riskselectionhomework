using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Homework.Api.Interfaces;
using Homework.Api.Models;
using Homework.Api.Services;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

/// <summary>
/// Tests for the ProductService class
/// <author>Rasmus Hyyppä</author>
/// </summary>
public class ProductServiceTests
{
    private readonly ProductService _productService;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

    public ProductServiceTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        var options = Options.Create(new ProductSourceOptions
        {
            ProductSourceUrls = new List<string> { "https://dummyjson.com/products" }
        });

        _productService = new ProductService(httpClient, options);
    }

    /// <summary>
    /// Tests that GetProductsAsync filters products by discount percentage (>= 10).
    /// </summary>
    [Fact]
    public async Task GetProductsAsync_ReturnsFilteredProducts()
    {
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"products\": [{ \"id\": 1, \"title\": \"Product 1\", \"brand\": \"Brand A\", \"price\": 20, \"discountPercentage\": 15, \"rating\": 4.5 } ]}")
        };
        
        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        var result = await _productService.GetProductsAsync();

        Assert.Single(result);
        Assert.Equal("Product 1", result[0].Title);
        Assert.True(result[0].DiscountPercentage >= 10);
    }

    /// <summary>
    /// Tests that GetProductsAsync returns an empty list when no products meet the discount criteria.
    /// </summary>
    [Fact]
    public async Task GetProductsAsync_ReturnsEmptyListWhenNoProductsMeetCriteria()
    {
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"products\": [{ \"id\": 2, \"title\": \"Product 2\", \"brand\": \"Brand B\", \"price\": 10, \"discountPercentage\": 5 } ]}")
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(mockResponse);

        var result = await _productService.GetProductsAsync();

        Assert.Empty(result);
    }

    /// <summary>
    /// Tests that GetProductsAsync throws an HttpRequestException when an invalid URL is used.
    /// </summary>
    [Fact]
    public async Task GetProductsAsync_ReturnsErrorOnInvalidUrl()
    {
      var mockResponse = new HttpResponseMessage(HttpStatusCode.NotFound);
      _httpMessageHandlerMock.Protected()
        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
        .ReturnsAsync(mockResponse);

      await Assert.ThrowsAsync<HttpRequestException>(() => _productService.GetProductsAsync());
    }

    /// <summary>
    /// Tests that GetTrendingProduct returns the product with the highest rating from a given list.
    /// </summary>
    /// <param name="products"> The list of products to evalutate.</param>
    /// <returns> Returns the product with the highest rating.
    [Fact]
    public void GetTrendingProduct_ReturnsProductWithHighestRating()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Title = "Product 1", Rating = 4.0f },
            new Product { Id = 2, Title = "Product 2", Rating = 4.8f }, 
            new Product { Id = 3, Title = "Product 3", Rating = 3.9f }
        };

        var trendingProduct = _productService.GetTrendingProduct(products);

        Assert.NotNull(trendingProduct);
        Assert.Equal(2, trendingProduct.Id);
        Assert.Equal("Product 2", trendingProduct.Title);
    }
}
