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
            try
            {
                var products = await _productService.GetProductsAsync();

                if (products == null || products.Count == 0)
                {
                    _logger.LogInformation("No products found.");
                    return NotFound("No products available.");
                }

                return Ok(products);
            }
            catch (ServiceUnavailableException ex)
            {
                _logger.LogError(ex, "Service is currently unavailable.");
                return StatusCode(503, "Service is currently unavailable. Please try again later.");
            }
            catch (JsonParseException ex)
            {
                _logger.LogError(ex, "Failed to parse products data.");
                return StatusCode(500, "An error occurred while processing product data.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching products.");
                return StatusCode(500, "An internal server error occurred.");
            }
        }
    }
}
