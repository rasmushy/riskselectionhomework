using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Homework.Api.Interfaces;
using Homework.Api.Models;
using Microsoft.Extensions.Logging;
using Homework.Api.Exceptions; 

namespace Homework.Api.Controllers
{

  [Route("api/[controller]")]
  [ApiController]
  public class ProductsController : ControllerBase
  {
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService productService, ILogger<ProductsController> logger)
    {
      _productService = productService;
      _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
      var products = await _productService.GetProductsAsync();

      if (products == null || products.Count == 0)
      {
        _logger.LogInformation("No products found.");
        return NotFound("No products available.");
      }

      return Ok(products);
    }
  }
}
