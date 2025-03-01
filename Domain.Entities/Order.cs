namespace Domain.Entities
{
    public class Order
    {
        public string Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }   
        public decimal Price { get; set; }  
        public string Status { get; set; }
    }
}
