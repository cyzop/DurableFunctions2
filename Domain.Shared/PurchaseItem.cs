namespace Domain.Shared
{
    public class PurchaseItem
    {
        public int Quantity { get; set; }
        public decimal UnityPrice { get; set; }
        public PurchaseProduct Product { get; set; }
    }
}
