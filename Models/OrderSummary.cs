namespace CbcRoastersErp.Models
{
    public struct OrderSummary
    {
        public int Placed { get; set; }
        public int Shipped { get; set; }
        public int AwaitingFulfillment { get; set; }
        public int AwaitingShipment { get; set; }
        public int AwaitingPayment { get; set; }
        public decimal TotalSalesAmount { get; set; }
    }
}
