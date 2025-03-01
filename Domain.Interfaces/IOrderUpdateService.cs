namespace Domain.Interfaces
{
    public interface IOrderUpdateService
    {
        Task UpdateOrderStatus(string orderId, string status);
    }
}
