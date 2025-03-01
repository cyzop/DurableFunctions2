using Azure.Messaging.ServiceBus;
using Domain.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;

namespace HttpDurableFunction
{
    public class OrchestrationFunction
    {
        private readonly ServiceBusClient _serviceBusClient;

        public OrchestrationFunction(ServiceBusClient serviceBusClient)
        {
            _serviceBusClient = serviceBusClient;
        }

        [Function(nameof(OrchestrationFunction))]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            try
            {
                ILogger logger = context.CreateReplaySafeLogger(nameof(OrchestrationFunction));

                var payload = context.GetInput<string>() ?? null;

                logger.LogInformation(payload ?? "Orchestrator, payload is received.");
                
                var data = JsonConvert.DeserializeObject<Order>(payload);
                await context.CallActivityAsync("ValidateOrder", data);

                await context.CallActivityAsync("ApproveOrder", data);

                logger.LogInformation("Orchestrator completed sucessfuul!");
            }
            catch(Exception ex)
            {
                string erro = ex.Message;
            }
            await Task.CompletedTask;
        }

        [Function("ValidateOrder")]
        public async Task ValidateOrder([ActivityTrigger] Order order, ILogger log)
        {
            log = log ?? NullLogger.Instance;

            log.LogInformation($"Validating order.");
            try
            {
                //regra de validação
                if (order.Id == null)
                    throw new Exception("Pedido sem identificação!");

                if(string.IsNullOrEmpty(order.ProductName))
                    throw new Exception("O pedido não contém produto!");

                if(order.Quantity < 1 || order.Quantity > 10)
                    throw new Exception("Pedido inválido, a quantidade solicita deve estar entre 1 e 10!");

                if(order.Price <=0)
                    throw new Exception("Pedido inválido, o preço informado não é válido!");

            }
            catch (Exception ex) {
                log.LogError($"Invalid order: {ex.Message}");
                throw ex;
            }
            await Task.CompletedTask;
        }


        [Function("ApproveOrder")]
        public async Task ApproveOrder([ActivityTrigger] Order order, ILogger log)
        {
            log = log ?? NullLogger.Instance;
            log.LogInformation($"Approving order.");
            try
            {
                //regra de aprovação
                log.LogInformation($"order = {order}");

                // Enviar o conteúdo para o Service Bus
                var sender = _serviceBusClient.CreateSender(Environment.GetEnvironmentVariable("ApprovedQueue"));
                //var sender = _serviceBusClient.CreateSender("orderqueue");

                order.Status = "Approved";

                var message = new ServiceBusMessage(JsonConvert.SerializeObject(order));
                await sender.SendMessageAsync(message);

            }
            catch (Exception ex)
            {
                log.LogError($"Approving failed: {ex.Message}");
                throw ex;
            }
            await Task.CompletedTask;
        }

        [Function("FunctionOrder_HttpStart")]
        public async Task<HttpResponseData> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
            [DurableClient] DurableTaskClient client,
            FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("FunctionOrder_HttpStart");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(OrchestrationFunction), requestBody);

            logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

            // Returns an HTTP 202 response with an instance management payload.
            // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
            return await client.CreateCheckStatusResponseAsync(req, instanceId);
        }
    }
}
