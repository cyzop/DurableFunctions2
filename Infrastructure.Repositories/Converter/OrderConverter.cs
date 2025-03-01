using Domain.Entities;
using Infrastructure.Repositories.Model;

namespace Infrastructure.Repositories.Converter
{
    public static class OrderConverter
    {
        public static OrderDataModel ConvertFromEntity(Order order)
        {
            return new OrderDataModel { Id = order.Id,
                Price = order.Price,
                ProductName = order.ProductName,
                Quantity = order.Quantity,
                Status = order.Status
            };
        }

        public static Order ConvertFromModel(OrderDataModel datamodel)
        {
            return new Order { Id = datamodel.Id, 
                Price = datamodel.Price, 
                Status = datamodel.Status, 
                Quantity = datamodel.Quantity, 
                ProductName = datamodel.ProductName };
        }
    }
}
