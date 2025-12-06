namespace SalesAnalytics.Application.Dtos
{
    public class DwhLoadDto
    {
        public List<CustomerDto>? Customers { get; set; }
        public List<ProductDto>? Products { get; set; }
        public List<DateDto>? Dates { get; set; }
        public List<FactSalesDto>? Sales { get; set; }
    }
}
