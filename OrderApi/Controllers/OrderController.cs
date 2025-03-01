using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Model;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        private readonly ILogger<OrderController> _logger;


        public OrderController(OrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderRequest orderrequest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var order = new Order
                    {
                        ProductName = orderrequest.ProductName,
                        Price = orderrequest.Price,
                        Quantity = orderrequest.Quantity,
                        Status = "Requested"
                    };

                    _logger.LogInformation($"CreateOrder {order.ProductName} | {order.Quantity} | {order.Price}");
                    await _orderService.AddOrderAsync(order);

                    return Ok(new OrderResponse(order.Id, order.Status, order.ProductName, order.Quantity, order.Price));
                }
                else
                {
                    var erros = ModelState.Values
                        .Where(x => x.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
                        .Select(x => x.Errors?.FirstOrDefault()?.ErrorMessage).ToList();
                    return BadRequest(new
                    {
                        PayloadErros = erros
                    });
                }
            }
            catch (Exception ex) { 
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            if (orders?.Count() > 0)
                return Ok(orders.Select(order => new OrderResponse(order.Id, order.Status, order.ProductName, order.Quantity, order.Price)));
            else
                return NoContent();
        }
    }
}
