using System.ComponentModel.DataAnnotations;

namespace SalesAnalytics.Application.Dtos
{
    public class FactSalesDto
    {
        public int SourceCustomerId { get; set; }
        public int SourceProductId { get; set; }
        public DateTime OrderDate { get; set; }
        public int Quantity { get; set; }
        public decimal Total_Venta { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
