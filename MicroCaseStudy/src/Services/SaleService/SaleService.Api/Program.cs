using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using SaleService.Api.Extensions.Registration.EventHandlerRegistration;
using SaleService.Api.IntegrationEvents.EventHandlers;
using SaleService.Api.IntegrationEvents.Events;
using SaleService.Api.ServiceRegistration;
using SaleService.Bootstrapper;

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

environment = builder.Environment;
builder.Host.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateOnBuild = false;
    options.ValidateScopes = false;
});
configureHostBuilder = builder.Host;
configuration = configurationBuilder.Build();
 

builder.Services.AddConfigureEventHandlers();

builder.Services.AddSingleton(sp =>
{
    EventBusConfig config = new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "SaleService",
        EventBusType = EventBusType.RabbitMQ,

    };

    return EventBusFactory.Create(config, sp);
});

builder.Services.AddSaleServiceBootstrapperServiceRegistration(configuration);
var app = builder.Build();

app.AddSaleServiceApiBuilderRegistration(app.Environment, configuration);

app.MapControllers(); 
 

var eventBus = app.Services.GetRequiredService<IEventBus>();

eventBus.Subscribe<SaleCreatedIntegrationEvent, SaleCreatedIntegrationEventHandler>();
eventBus.Subscribe<CustomerUpdatedIntegrationEvent, CustomerCreatedIntegrationEventHandler>();
app.Run();