namespace Domain.Shared
{
    public class PurchaseOrder
    {
        public string OrderId { get; set; }
        public List<PurchaseItem> Items { get; set; }
    }
}
