using Domain.Interfaces;

namespace Application.Services
{
    public class OrderUpdateService : IOrderUpdateService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderUpdateService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task UpdateOrderStatus(string orderId, string status)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if(order != null)
            {
                order.Status = status;
                await _orderRepository.UpdateOrderAsync(order);
            }
        }
    }
}
