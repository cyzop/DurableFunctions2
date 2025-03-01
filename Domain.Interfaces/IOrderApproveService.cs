using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IOrderApproveService
    {
        Task TriggerOrderApprover(Order order);
    }
}
