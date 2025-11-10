using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesAnalytics.Domain.Entities.Csv
{
    public class Sale
    {
        // Properties of Order
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateOnly OrderDate { get; set; }
        public string? Status { get; set; }

        // Properties of OrderDetail
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
