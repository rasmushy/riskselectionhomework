using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Homework.Api.Interfaces;
using Homework.Api.Models;
using Homework.Api.Exceptions;

namespace Homework.Api.Infrastructure.ParsingStrategies
{
    public class DummyJsonParsingStrategy : IApiResponseParsingStrategy
    {
        public async Task<List<Product>> ParseResponseAsync(HttpResponseMessage response)
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
    }
}
