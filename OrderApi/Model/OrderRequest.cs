namespace OrderApi.Model
{
    public class OrderRequest
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public OrderRequest(string productName, int quantity, decimal price)
        {
            ProductName = productName;
            Quantity = quantity;
            Price = price;
        }
    }
}
