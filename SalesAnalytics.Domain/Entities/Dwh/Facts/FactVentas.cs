using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesAnalytics.Domain.Entities.Dwh.Facts
{
    [Table("FactVentas", Schema = "Fact")]
    public class FactVentas
    {
        [Key]
        public int Venta_Id { get; set; }
        public int FK_Product { get; set; }
        public int FK_Customer { get; set; }
        public int FK_Date { get; set; }
        public int Quantity { get; set; }
        public decimal Total_Venta { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
