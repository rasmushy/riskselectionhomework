using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Homework.Api.Models;
using Homework.Api.Exceptions;

namespace Homework.Api.Services
{
    /// <summary>
    /// Processes HTTP responses and deserializes them into product objects, 
    /// as used by <see cref="ProductApiClient"/>.
    /// </summary>
    public static class ApiResponseHandler
    {
        public static async Task<List<Product>> HandleResponse(HttpResponseMessage response)
        {
            try 
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(jsonString);

                if (!jsonDoc.RootElement.TryGetProperty("products", out var productsElement) || productsElement.ValueKind != JsonValueKind.Array)
                {
                    throw new JsonParseException("Expected 'products' array is missing or malformed in the JSON response.");
                }

                var products = productsElement.Deserialize<List<Product>>(new JsonSerializerOptions
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
