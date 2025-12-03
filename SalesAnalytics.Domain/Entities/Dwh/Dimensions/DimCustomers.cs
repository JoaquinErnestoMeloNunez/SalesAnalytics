using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesAnalytics.Domain.Entities.Dwh.Dimensions
{
    [Table("Dim_Customers", Schema = "Dimension")]
    public class DimCustomers
    {
        [Key]
        public int Customer_Key { get; set; }
        public int? Customer_Id { get; set; }
        public string? Customer_Name { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
