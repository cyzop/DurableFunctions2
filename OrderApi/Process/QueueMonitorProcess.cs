using Azure.Messaging.ServiceBus;
using Domain.Entities;
using Domain.Interfaces;
using System.Text.Json;


namespace OrderApi.Process
{
    public class QueueMonitorProcess : BackgroundService
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;

        public QueueMonitorProcess(string connectionString, string queueName, IServiceProvider provider, ILogger logger)
        {
            _client = new ServiceBusClient(connectionString);
            _processor = _client.CreateProcessor(queueName, new ServiceBusProcessorOptions());
            _provider = provider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;

            await _processor.StartProcessingAsync(stoppingToken);
            _logger.LogInformation("Process started...");
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            try
            {
                _logger.LogInformation($"Processing Message: {args.Message.MessageId} | Body: {body}");
                var order = JsonSerializer.Deserialize<Order>(body);

                using (var scope = _provider.CreateScope())
                {
                    var orderService = scope.ServiceProvider.GetRequiredService<IOrderUpdateService>();

                    await orderService.UpdateOrderStatus(order.Id, order.Status);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError($"Message error: {ex.Message} | Message: {args.Message.MessageId} | Body: {body}");
            }
            //retira da fila (mesmo se estiver com erro)
            await args.CompleteMessageAsync(args.Message);
        }
        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Message error: {args.Exception}.");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await _processor.StopProcessingAsync(stoppingToken);
            await _processor.DisposeAsync();
            await _client.DisposeAsync();
        }
    }
}
