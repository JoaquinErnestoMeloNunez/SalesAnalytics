namespace SalesAnalytics.Application.Dtos
{
    public class DimDtos
    {
        public List<CustomerDto>? Customers { get; set; }
        public List<ProductDto>? Products { get; set; }
        public List<DateDto>? Dates { get; set; }
    }
}
