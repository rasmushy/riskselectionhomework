using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Homework.Api.Exceptions;
using Homework.Api.Models;
using Homework.Api.Services;
using Xunit;

namespace Homework.Api.Tests.Services
{
  /// <summary>
  /// Unit tests for the <see cref="ApiResponseHandler"/> class.
  /// </summary>
  /// <author>Rasmus Hyyppä</author>
  public class ApiResponseHandlerTests
  {
    /// <summary>
    /// Verifies that <see cref="ApiResponseHandler.HandleResponse"/> returns a list of products when the JSON data is valid.
    /// </summary>
    [Fact]
    public async Task HandleResponse_ReturnsProductsList_WhenJsonIsValid()
    {
      var json = "{\"products\": [{ \"id\": 1, \"title\": \"Product\", \"brand\": \"Brand A\", \"price\": 20, \"discountPercentage\": 15 }]}";
      var response = new HttpResponseMessage
      {
        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
      };

      var result = await ApiResponseHandler.HandleResponse(response);
      Assert.Single(result);
      Assert.Equal("Product", result[0].Title);
    }

    /// <summary>
    /// Ensures that <see cref="ApiResponseHandler.HandleResponse"/> throws a <see cref="JsonParseException"/> if the JSON data is malformed.
    /// </summary>
    [Fact]
    public async Task HandleResponse_ThrowsJsonParseException_WhenJsonIsMalformed()
    {
      var json = "{ malformed_json: true }";
      var response = new HttpResponseMessage
      {
        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
      };

      await Assert.ThrowsAsync<JsonParseException>(() => ApiResponseHandler.HandleResponse(response));
    }

    /// <summary>
    /// Verifies that <see cref="ApiResponseHandler.HandleResponse"/> returns an empty list when the JSON 'products' array is empty.
    /// </summary>
    [Fact]
    public async Task HandleResponse_ReturnsEmptyList_WhenProductsArrayIsEmpty()
    {
      var json = "{\"products\": []}";
      var response = new HttpResponseMessage
      {
        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
      };

      var result = await ApiResponseHandler.HandleResponse(response);
      Assert.Empty(result);
    }

    /// <summary>
    /// Ensures that <see cref="ApiResponseHandler.HandleResponse"/> throws a <see cref="JsonParseException"/> if the 'products' property is missing.
    /// </summary>
    [Fact]
    public async Task HandleResponse_ThrowsJsonParseException_WhenProductsPropertyIsMissing()
    {
      var json = "{\"items\": []}";
      var response = new HttpResponseMessage
      {
        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
      };

      await Assert.ThrowsAsync<JsonParseException>(() => ApiResponseHandler.HandleResponse(response));
    }

    /// <summary>
    /// Ensures that <see cref="ApiResponseHandler.HandleResponse"/> throws a <see cref="JsonParseException"/> if the 'products' property is not an array.
    /// </summary>
    [Fact]
    public async Task HandleResponse_ThrowsJsonParseException_WhenProductsPropertyIsNotArray()
    {
      var json = "{\"products\": { \"id\": 1, \"title\": \"Invalid Product\" }}";
      var response = new HttpResponseMessage
      {
        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
      };

      await Assert.ThrowsAsync<JsonParseException>(() => ApiResponseHandler.HandleResponse(response));
    }
  }
}
