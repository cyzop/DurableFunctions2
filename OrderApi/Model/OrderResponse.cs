namespace OrderApi.Model
{
    public class OrderResponse : OrderRequest
    {
        public string Id { get; set; }
        public string Status { get; set; }

        public OrderResponse(string id, string status, string productName, int quantity, decimal price) : base(productName, quantity, price)
        {
            Id = id;
            Status = status;
        }
    }
}
