using System.Collections.Generic;
using System.Threading.Tasks;
using Homework.Api.Models;

namespace Homework.Api.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
    }
}
