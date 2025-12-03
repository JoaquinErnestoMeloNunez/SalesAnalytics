using Microsoft.EntityFrameworkCore;
using SalesAnalytics.Api.Data.Context;
using SalesAnalytics.Api.Data.Entities;
using SalesAnalytics.Api.Data.Interface;

namespace SalesAnalytics.Api.Data.Repositories
{
    public class ProductApiRepository : IProductApiRepository
    {
        private readonly ProductContext _context;
        public ProductApiRepository(ProductContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }
    }
}
