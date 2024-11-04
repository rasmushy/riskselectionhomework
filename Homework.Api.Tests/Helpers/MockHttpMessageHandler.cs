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
    /// MockHttpMessageHandler simulates HTTP responses for ProductService.
    /// </summary>
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly bool _returnEmptyList;

        /// <param name="returnEmptyList">Determines whether to return an empty list of products.</param>
        public MockHttpMessageHandler(bool returnEmptyList = false)
        {
            _returnEmptyList = returnEmptyList;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

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
