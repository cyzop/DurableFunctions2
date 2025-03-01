using Application.Services;
using Domain.Interfaces;
using Infrastructure.Repositories;
using OrderApi.Process;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.ConfigureServices(services =>
{
    services.AddScoped<OrderService>();
    services.AddScoped<IOrderRepository, OderRepository>();
    services.AddScoped<IOrderApproveService, OrderApproveService>();
    services.AddScoped<IOrderUpdateService, OrderUpdateService>();

    services.AddHostedService(provider =>
    {
        var logger = provider.GetRequiredService<ILogger<QueueMonitorProcess>>();
        return new QueueMonitorProcess(Environment.GetEnvironmentVariable("ServiceBusConnection"),
                Environment.GetEnvironmentVariable("ApprovedQueue"), provider, logger);
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
