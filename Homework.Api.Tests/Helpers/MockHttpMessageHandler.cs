using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Homework.Api.Models;

namespace Homework.Api.Tests.Helpers
{
  /// <summary>
  /// MockHttpMessageHandler simulates HTTP responses for API client tests.
  /// Allows configuration of empty, valid, or malformed responses.
  /// </summary>
  public class MockHttpMessageHandler : HttpMessageHandler
  {
    private readonly bool _returnEmptyList;
    private readonly bool _returnMalformedJson;

    public MockHttpMessageHandler(bool returnEmptyList = false, bool returnMalformedJson = false)
    {
      _returnEmptyList = returnEmptyList;
      _returnMalformedJson = returnMalformedJson;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      await Task.CompletedTask;

      if (_returnMalformedJson)
      {
        return new HttpResponseMessage(HttpStatusCode.OK)
        {
          Content = new StringContent("{ malformed_json: true }", System.Text.Encoding.UTF8, "application/json")
        };
      }

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
