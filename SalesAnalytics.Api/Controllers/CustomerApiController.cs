using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesAnalytics.Persistence.Repositories.Api;

namespace SalesAnalytics.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerApiController : ControllerBase
    {
        private readonly ICustomerApiRespository customerApiRespository;

        public CustomerApiController(ICustomerApiRespository customerApiRespository)
        {
            this.customerApiRespository = customerApiRespository;
        }

        [HttpGet("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await customerApiRespository.GetCustomersAsync();
            return Ok(customers);
        }
    }
}
