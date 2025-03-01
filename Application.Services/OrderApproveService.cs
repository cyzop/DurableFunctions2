using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace Application.Services
{
    public class OrderApproveService : IOrderApproveService
    {
        private readonly ILogger<OrderApproveService> _logger;

        public OrderApproveService(ILogger<OrderApproveService> logger)
        {
            _logger = logger;
        }

        public async Task TriggerOrderApprover(Order order)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var url = Environment.GetEnvironmentVariable("approveazfunction");
                    var json = JsonConvert.SerializeObject(order);
                    StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    response.EnsureSuccessStatusCode();
                    string responsebody = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Approve trigger failed:{ex.Message}");
                }
            }
        }
    }
}
