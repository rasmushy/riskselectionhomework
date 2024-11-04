using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Homework.Api.Models;
using Homework.Api.Exceptions;

namespace Homework.Api.Services
{
  public static class ApiResponseHandler
  {
    public static async Task<List<Product>> HandleResponse(HttpResponseMessage response)
    {
      try 
      {
        var jsonString = await response.Content.ReadAsStringAsync();
        var jsonDoc = JsonDocument.Parse(jsonString);

        var products = jsonDoc.RootElement.GetProperty("products").Deserialize<List<Product>>(new JsonSerializerOptions
            {
            PropertyNameCaseInsensitive = true
            });

        return products ?? new List<Product>();
      }
      catch (JsonException ex) 
      {
        throw new JsonParseException("Failed to parse the product JSON response.", ex);
      }
    }
  }
}
