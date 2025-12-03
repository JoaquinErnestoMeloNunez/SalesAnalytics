using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesAnalytics.Domain.Entities.Dwh.Dimensions
{
    [Table("Dim_Products", Schema = "Dimension")]
    public class DimProducts
    {
        [Key]
        public int Product_Key { get; set; }
        public int Product_Id { get; set; }
        public string Product_Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public decimal List_Price { get; set; }
    }
}
