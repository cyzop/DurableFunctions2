using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderApproveService _approveService;

        public OrderService(IOrderRepository orderRepository, IOrderApproveService approveService)
        {
            _orderRepository = orderRepository;
            _approveService = approveService;
        }

        public async Task<Order> GetOrderByIdAsync(string id)
        {
            return await _orderRepository.GetOrderByIdAsync(id);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync();
        }

        public async Task AddOrderAsync(Order order)
        {
            order.Status = "requested";

            //adiciona a order local
            var persistendOrder = await _orderRepository.AddOrderAsync(order);

            //inicia processo e aprovação 
            _approveService.TriggerOrderApprover(persistendOrder);
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await _orderRepository.UpdateOrderAsync(order);
        }
    }
}
