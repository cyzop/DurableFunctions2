using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Repositories.Converter;
using Infrastructure.Repositories.Model;
using MongoDB.Driver;

namespace Infrastructure.Repositories
{
    public class OderRepository : IOrderRepository
    {

        private readonly IMongoCollection<OrderDataModel> _orders;

        public OderRepository()
        {
            string connectionString = Environment.GetEnvironmentVariable("MongoConnectionstring");
            var mongoClient = new MongoClient(connectionString);
            var mongoDataBase = mongoClient.GetDatabase("OrderDB");
            _orders = mongoDataBase.GetCollection<OrderDataModel>("Orders");
        }

        public Task<Order> AddOrderAsync(Order order)
        {
            var model = OrderConverter.ConvertFromEntity(order);
            _orders.InsertOne(model);
            order.Id = model.Id;
            return Task.FromResult(order);
        }

        public Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var dbItems = _orders.Find(a => true).ToList();
            return Task.FromResult(dbItems.Select(e => OrderConverter.ConvertFromModel(e))?.AsEnumerable());
        }

        public Task<Order> GetOrderByIdAsync(string id)
        {
            var model = _orders.Find(a => a.Id == id).FirstOrDefault();
            return Task.FromResult(model != null ? OrderConverter.ConvertFromModel(model) : null);
        }

        public Task UpdateOrderAsync(Order order)
        {
            var orderModel = OrderConverter.ConvertFromEntity(order);
            var existingOrder = GetOrderByIdAsync(orderModel.Id).Result;
            if (existingOrder != null)
            {
                existingOrder.ProductName = orderModel.ProductName;
                existingOrder.Quantity = orderModel.Quantity;
                existingOrder.Price = orderModel.Price;
                existingOrder.Status = orderModel.Status;

                _orders.ReplaceOne(p => p.Id == orderModel.Id, orderModel);
            }
            else
                AddOrderAsync(order);

            return Task.CompletedTask;
        }

        public Task UpdateOrderStatus(Order order)
        {
            var orderModel = OrderConverter.ConvertFromEntity(order);
            var existingOrder = GetOrderByIdAsync(orderModel.Id).Result;
            if (existingOrder != null)
            {
                existingOrder.Status = orderModel.Status;
                _orders.ReplaceOne(p => p.Id == orderModel.Id, orderModel);
            }
            return Task.CompletedTask;
        }
    }
}
