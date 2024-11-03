using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homework.Api.Interfaces;
using Homework.Api.Models;

namespace Homework.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
          try
          {
            var products = await _productService.GetProductsAsync() ?? new List<Product>();
            return Ok(products);
          }
          catch (Exception)
          {
            return StatusCode(500, "An error occurred while fetching products.");
          }
        }
    }
}
