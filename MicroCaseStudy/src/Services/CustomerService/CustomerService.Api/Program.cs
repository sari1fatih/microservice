using CustomerService.Api.ServiceRegistration;
using CustomerService.Bootstrapper;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.RabbitMQ;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration;
ConfigureHostBuilder configureHostBuilder;
IWebHostEnvironment environment;

IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
// Add services to the container.

builder.Host.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateOnBuild = false;
    options.ValidateScopes = false;
});
environment = builder.Environment;
configureHostBuilder = builder.Host;
configuration = configurationBuilder.Build();

RabbitmqConfig rabbitmqConfig = configuration.GetSection(nameof(RabbitmqConfig)).Get<RabbitmqConfig>();

builder.Services.AddSingleton<IEventBus>(sp =>
{
    EventBusConfig config = new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "SaleService",
        EventBusType = EventBusType.RabbitMQ,
        Connection = new ConnectionFactory()
        {
            HostName = rabbitmqConfig?.HostName
        }
    };

    return EventBusFactory.Create(config, sp);
});
builder.Services.AddCustomerServiceBootstrapperServiceRegistration(configuration);
var app = builder.Build();

app.AddCustomerServiceApiBuilderRegistration(app.Environment, configuration);

app.MapControllers();
 

app.Run();