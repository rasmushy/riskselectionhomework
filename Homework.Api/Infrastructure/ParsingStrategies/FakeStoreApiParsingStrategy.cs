using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Homework.Api.Interfaces;
using Homework.Api.Models;

namespace Homework.Api.Infrastructure.ParsingStrategies
{
    public class FakeStoreApiParsingStrategy : IApiResponseParsingStrategy
    {
        public async Task<List<Product>> ParseResponseAsync(HttpResponseMessage response)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var rawProducts = JsonSerializer.Deserialize<List<RawFakeStoreProduct>>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (rawProducts == null)
            {
                return new List<Product>();
            }

            return rawProducts.Select(rawProduct => new Product
            {
                Id = rawProduct.Id,
                Title = rawProduct.Title,
                Price = rawProduct.Price,
                Description = rawProduct.Description,
                Brand = ExtractBrand(rawProduct.Title),
            }).ToList();
        }

        private string ExtractBrand(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return string.Empty;
            }

            // Assume the brand is the first word(s) before a hyphen
            var brand = title.Split('-').FirstOrDefault()?.Trim();
            return brand ?? string.Empty;
        }
    }

    class RawFakeStoreProduct
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Image { get; set; }
        public RawRating? Rating { get; set; }
    }

    class RawRating
    {
        public double Rate { get; set; }
        public int Count { get; set; }
    }
}
