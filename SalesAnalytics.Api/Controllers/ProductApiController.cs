using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesAnalytics.Api.Data.Interface;

namespace SalesAnalytics.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductApiController : ControllerBase
    {
        private readonly IProductApiRepository productApiRepository;

        public ProductApiController(IProductApiRepository productApiRepository)
        {
            this.productApiRepository = productApiRepository;
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await productApiRepository.GetProductsAsync();
            return Ok(products);
        }
    }
}
