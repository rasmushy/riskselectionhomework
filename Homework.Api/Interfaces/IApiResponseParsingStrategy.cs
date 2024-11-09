using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Homework.Api.Models;

namespace Homework.Api.Interfaces
{
    public interface IApiResponseParsingStrategy
    {
        Task<List<Product>> ParseResponseAsync(HttpResponseMessage response);
    }
}
