using System.Collections.Generic;
using System.Threading.Tasks;
using Homework.Api.Models;

namespace Homework.Api.Interfaces
{
  public interface IProductApiClient
  {
    Task<List<Product>> FetchProductsAsync();
  }
}

