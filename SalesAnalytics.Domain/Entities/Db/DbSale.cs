namespace SalesAnalytics.Domain.Entities.Db
{
    public class DbSale
    {
        public int OrderID { get; set; }
        public int CustomerID { get; set; }
        public DateOnly OrderDate { get; set; }
        public string? Status { get; set; }

        // Properties of OrderDetail
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal{ get; set; }
        public int SourceID { get; set; }

    }
}
