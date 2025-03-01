using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(string id);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> AddOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task UpdateOrderStatus(Order order);
    }
}
