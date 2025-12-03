namespace SalesAnalytics.Application.Dtos
{
    public class ProductDto
    {
        public int ProductID { get; set; }
        public string? ProductName { get; set; }
        public string? Category { get; set; }
        public decimal ListPrice { get; set; }
    }
}
